using BuffetAdminMVC.Models;
using BuffetAuxiliaryLib.BLL;
using BuffetDAL.AdditionalModels;
using BuffetDAL.Enumerations;
using BuffetDAL.Models;
using BuffetDAL.Repos.ADO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAdminMVC.Services
{
    public class AdminService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ADOUnitOfWork uow;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(ADOUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this.uow = unitOfWork;
            this._userManager = userManager;
            this._signInManager = signInManager;

            this._httpContextAccessor = httpContextAccessor;
        }

        public List<User> GetAdmins()
        {
            List<User> admins = new List<User>(this.uow.Admin.GetUsersInRole((int)Roles.SA));
            foreach (var user in this.uow.Admin.GetUsersInRole((int)Roles.Admin))
            {
                admins.Add(user);
            }
            return admins;
        }

        public bool IsEmailNotExist(string credentialEmail)
        {
            return _userManager.FindByEmailAsync(credentialEmail).Result == null && this.uow.Users.Read(credentialEmail) == null;
        }

        public async Task<IdentityResult> CreateIdentityUser(RegisterViewModel model)
        {
            IdentityUser identityUser = new IdentityUser { Email = model.Email, UserName = model.Email };
            return await _userManager.CreateAsync(identityUser, model.Password);
        }

        public async Task AddIdentityUserToRole(RegisterViewModel model)
        {
            IdentityUser addedUser = await _userManager.FindByNameAsync(model.Email);
            await _userManager.AddToRoleAsync(addedUser, role: "Admin");
        }

        public void CreateUserAndAddToDb(RegisterViewModel model)
        {
            User user = new User { Email = model.Email, Name = model.Name, Surname = model.Surname, RoleId = (int)Roles.Admin };
            this.uow.Users.Create(user);
        }

        /// <summary>
        /// Sets admin role to user role value.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAdmin(int id)
        {
            User user = this.uow.Users.Read(id);
            user.RoleId = (int)Roles.User;
            this.uow.Users.Update(user);

            IdentityUser identityUser = await _userManager.FindByNameAsync(user.Email);
            await _userManager.RemoveFromRoleAsync(identityUser, role: "Admin");
            await _userManager.AddToRoleAsync(identityUser, role: "User");
        }

        public List<Category> GetAllCategories()
        {
            return new List<Category>(this.uow.Categories.ReadAll());
        }

        public List<Food> GetAllFoods()
        {
            return new List<Food>(this.uow.Foods.ReadAll());
        }

        public Food SetCategoryForFood(Food food, string categoryName)
        {
            food.CategoryId = uow.Categories.Read(categoryName).Id;
            return food;
        }

        public bool IsFoodUnique(Food food)
        {
            List<Food> foodsList = this.GetAllFoods();
            foreach (Food fd in foodsList)
            {
                if (String.Equals(food.Name, fd.Name))
                {
                    return false;
                }
            }
            return true;
        }
        
        public void AddFoodToDb(Food food)
        {
            this.uow.Foods.Create(food);
        }

        public List<Menu> GetAllMenus()
        {
            return new List<Menu>(this.uow.Menus.ReadAll());
        }

        public Menu ReadMenuById(int id)
        {
            return this.uow.Menus.Read(id);
        }

        public DataTable ReadMenuForDayAsDataTable(int id)
        {
            return this.uow.Admin.ReadMenuForDay(id);
        }

        public bool IsArrayEmpty(string[] array)
        {
            return array.Length == 0;
        }

        public DateTime ConvertFromStringToDateTime(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public bool IsDateNotInValidRange(DateTime menuDateTime)
        {
            return menuDateTime < DateTime.Now.AddDays(1).Date || menuDateTime > DateTime.Now.AddDays(7).Date;
        }

        public Menu ReadMenuForDate(DateTime menuDateTime)
        {
            return this.uow.Menus.Read(menuDateTime);
        }

        public List<MenuRowModel> FillMenuRows(string[] foods, string[] price, string[] baseAmount)
        {
            List<MenuRowModel> menuRows = new List<MenuRowModel>();

            int menuLength = foods.Length;
            decimal[] priceDecimal = ArraysConvert.FromStringToDecimal(price);
            int?[] baseAmountInt = ArraysConvert.FromStringToNullableInt(baseAmount);

            for (int i = 0; i < menuLength; i++)
            {
                menuRows.Add(new MenuRowModel()
                {
                    FoodName = foods[i],
                    Price = priceDecimal[i],
                    BaseAmount = baseAmountInt[i]
                });
            }

            return menuRows;
        }

        public bool IsThereDuplicatesInMenuRows(List<MenuRowModel> menuRows, MenuRowModel row)
        {
            int duplicatesCounter = 0;                                     
            foreach (MenuRowModel rowToCompare in menuRows)
            {
                if (row.FoodName.Equals(rowToCompare.FoodName))
                {
                    duplicatesCounter++;
                }
                if (duplicatesCounter > 1)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void CreateMenuWithList(DateTime menuDateTime, List<MenuRowModel> menuRows)
        {
            Menu menu = new Menu() { Date = menuDateTime };
            this.uow.Menus.Create(menu);

            List<MenuFood> listOfMenuFoods = new List<MenuFood>();
            foreach (MenuRowModel menuRow in menuRows)
            {
                listOfMenuFoods.Add(new MenuFood()
                {
                    MenuId = this.uow.Menus.Read(menuDateTime).Id,
                    FoodId = this.uow.Foods.Read(menuRow.FoodName).Id,
                    Price = menuRow.Price,
                    BaseAmount = menuRow.BaseAmount ?? 0,
                    AvailableAmount = menuRow.BaseAmount ?? 0,
                    InsufficientAmount = 0,
                });
            }
            foreach (MenuFood mf in listOfMenuFoods)
            {
                this.uow.MenuFoods.Create(mf);
            }
        }

        public bool IsPriceOrBaseAmountNotValid(List<MenuRowModel> menuRows)
        {
            foreach (MenuRowModel row in menuRows)
            {
                if (row.Price < 0 || row.BaseAmount <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsThereDuplicatesInMenuRows(List<MenuRowModel> menuRows)
        {
            foreach (MenuRowModel row in menuRows)
            {
                if (this.IsThereDuplicatesInMenuRows(menuRows, row))
                {
                    return true;
                }
            }
            return false;
        }

        public List<MenuUpdateModel> ReadMenuForDayAsList(int id)
        {
            return this.uow.Admin.ReadMenuForDayAsList(id);
        }

        public void UpdateMenuFoodsAmountForTomorrow(string[] id, string[] baseAmount, string menuId)
        {
            int mId;
            int.TryParse(menuId, out mId);

            int?[] baseAmountNullable = ArraysConvert.FromStringToNullableInt(baseAmount);
            int[] idInt = ArraysConvert.FromStringToInt(id);

            MenuUpdateModel[] menuDetails = this.uow.Admin.ReadMenuForDayAsList(mId).ToArray();

            for (int i = 0; i < menuDetails.Length; i++)
            {
                if (idInt[i] == menuDetails[i].Id)
                {
                    menuDetails[i].Base = baseAmountNullable[i];
                }
            }

            MenuFood[] menuFoods = new MenuFood[idInt.Length];

            for (int i = 0; i < menuFoods.Length; i++)
            {
                menuFoods[i] = this.uow.MenuFoods.Read(idInt[i]);
                menuFoods[i].BaseAmount = menuDetails[i].Base;
                menuFoods[i].AvailableAmount = menuDetails[i].Base;
            }

            foreach (MenuFood mf in menuFoods)
            {
                this.uow.MenuFoods.Update(mf);
            }
        }

        public DataTable CreateReportTable(string reportType, out string reportMessage)
        {
            DataTable report = new DataTable();
            switch (reportType)
            {
                case "dailyReserves":
                    reportMessage = "Daily reserves rating";
                    report = this.uow.SpecificReports.GetDailyReservesRating();
                    break;
                case "lackOfFoods":
                    reportMessage = "Food which was not enough";
                    report = this.uow.SpecificReports.GetLackOfFoods();
                    break;
                case "mostFavouriteFoods":
                    reportMessage = "Favourite foods";
                    report = this.uow.SpecificReports.GetMostFavouriteFoods();
                    break;
                case "mostPopularFoods":
                    reportMessage = "Most popular ordered foods";
                    report = this.uow.SpecificReports.GetMostPopularFoods();
                    break;
                default:
                    reportMessage = "Choose report type from dropdownlist";
                    break;
            }
            return report;
        }

        public List<FeedbackViewModel> GetAllFeedbacks()
        {
            Feedback[] feedbacks = this.uow.Feedbacks.ReadAll().ToArray<Feedback>();
            List<FeedbackViewModel> feedbacksViewModels = new List<FeedbackViewModel>();
            for (int i = 0; i < feedbacks.Length; i++)
            {
                feedbacksViewModels.Add(new FeedbackViewModel
                {
                    UserName = feedbacks[i].UserId != null ? this.uow.Users.Read((int)feedbacks[i].UserId).Email : "Anonymous",
                    Message = feedbacks[i].Message
                });
            }
            return feedbacksViewModels;
        }

        public List<ReserveModel> GetReservesWithUserNames()
        {
            return this.uow.Admin.GetReservesWithUserNames();
        }

        public ReserveModel GetReserveWithUserName(int id)
        {
            return this.uow.Admin.GetReserveWithUserName(id);
        }

        public List<ReserveDetailsModel> GetReserveDetails(int id)
        {
            return this.uow.Admin.GetReserveDetails(id);
        }

        public void AcceptReserve(int reserveId)
        {
            Reserve reserve = this.uow.Reserves.Read(reserveId);
            reserve.IsAccepted = true;
            this.uow.Reserves.Update(reserve);
        }

        public bool IsTimeForReserveExpired(int reserveId)
        {
            Reserve reserve = this.uow.Reserves.Read(reserveId);
            if((DateTime.Now - reserve.DateTime).TotalSeconds > 600 && reserve.IsAccepted != true)
            {
                return true;
            }

            return false;
        }

        public void DeclineReserve(int reserveId)
        {
            Reserve reserve = this.uow.Reserves.Read(reserveId);
            if(reserve.IsAccepted == null)
            {
                reserve.IsAccepted = false;
                this.uow.Reserves.Update(reserve);
                this.IncrementMenuFoodsAmountForReserve(reserve.Id);
            }
        }

        public void DeclineReserve(Reserve reserve)
        {
            if(reserve.IsAccepted == null)
            {
                reserve.IsAccepted = false;
                this.uow.Reserves.Update(reserve);
                this.IncrementMenuFoodsAmountForReserve(reserve.Id);
            }
        }

        private void IncrementMenuFoodsAmountForReserve(int reserveId)
        {
            List<MenuFoodReserve> reserveRows = (List<MenuFoodReserve>)this.uow.MenuFoodReserves.ReadAllForReserveId(reserveId);
            foreach (MenuFoodReserve mfr in reserveRows)
            {
                MenuFood mf = uow.MenuFoods.Read(mfr.MenuFoodId);                
                mf.AvailableAmount = mf.AvailableAmount + mfr.Amount;            
                uow.MenuFoods.Update(mf);                                        
            }
        }

        public void CheckReserves()
        {
            List<Reserve> reserves = (List<Reserve>)uow.Reserves.ReadAllNull(); 
            foreach(Reserve reserve in reserves)
            {
                if (IsTimeForReserveExpired(reserve.Id))
                {
                    DeclineReserve(reserve);
                }
            }
        }

        public bool IsUserInSAAdminRole(string userEmail)
        {
            User user = this.uow.Users.Read(userEmail);
            if(user != null)
            {
                return user.RoleId == (int)Roles.SA || user.RoleId == (int)Roles.Admin;
            }
            return false;
        }

        public async Task<SignInResult> ValidateAndAuthenticate(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(email, password, true, false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public UserViewModel GetUserProfileInfo()
        {
            User user = this.uow.Users.Read(_httpContextAccessor.HttpContext.User.Identity.Name);
            Role userRole = this.uow.Roles.Read(user.RoleId);
            UserViewModel userModel = new UserViewModel { Email = user.Email, Name = user.Name, Surname = user.Surname, RoleName = userRole.Name };
            return userModel;
        }

        public void UpdateUserProfileInfo(UserViewModel updatedUserModelInfo)
        {
            User userToUpdate = this.uow.Users.Read(_httpContextAccessor.HttpContext.User.Identity.Name);
            userToUpdate.Name = updatedUserModelInfo.Name;          //список полей которые могут быть изменены + добавить изменение пароля
            userToUpdate.Surname = updatedUserModelInfo.Surname;
            this.uow.Users.Update(userToUpdate);
        }


    }
}
