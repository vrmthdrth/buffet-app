using BuffetAuxiliaryLib.DTOs;
using BuffetDAL.Enumerations;
using BuffetDAL.Models;
using BuffetDAL.Repos.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BuffetWebAPI.Services
{
    public class ApiService
    {
        private readonly SecurityKeyService _securityKeyService;
        private readonly EFContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApiService(
            EFContext context,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            SecurityKeyService securityKeyService,
            IHttpContextAccessor contextAccessor) 
        {
            _securityKeyService = securityKeyService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        #region Authentication

        public string GenerateJwtToken(string credentialEmail)
        {
            var jwt = new JwtSecurityToken(
                            expires: DateTime.Now.Add(TimeSpan.FromDays(30)),
                            claims: new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, credentialEmail) },
                            signingCredentials: new SigningCredentials(
                                                    key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_securityKeyService.GetSecurityKey())), 
                                                    algorithm: SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public bool IsUserInUserRole(string userEmail)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if(user != null)
            {
                return user.RoleId == (int)Roles.User;
            }
            return false;
        }

        public async Task<SignInResult> ValidateCredentialsForLogin(LoginDTO loginInfoDTO)
        {
            return await _signInManager.PasswordSignInAsync(loginInfoDTO.Email, loginInfoDTO.Password, isPersistent: true, lockoutOnFailure: false);
        }

        public bool IsEmailNotExist(string credentialEmail)
        {
            return _userManager.FindByEmailAsync(credentialEmail).Result == null && this._context.Users.FirstOrDefault(u => u.Email == credentialEmail) == null;
        }

        public async Task<IdentityResult> AddNewUserToDataBase(RegisterDTO registerDTO) 
        {
            IdentityUser identityUser = new IdentityUser { Email = registerDTO.Email, UserName = registerDTO.Email };
            var result = await _userManager.CreateAsync(identityUser, registerDTO.Password);
            if (result.Succeeded)
            {
                IdentityUser addedUser = await _userManager.FindByNameAsync(registerDTO.Email);
                await _userManager.AddToRoleAsync(addedUser, "User");
                User user = new User { Email = registerDTO.Email, Name = registerDTO.Name, Surname = registerDTO.Surname, RoleId = (int)Roles.User };
                this._context.Users.Add(user);
                this._context.SaveChanges();
            }
            return result;
        }

        public async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ValidateAndChangePassword(ChangePasswordDTO changePassword) // PasswordChange
        {
            IdentityUser user = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
            if(await _userManager.CheckPasswordAsync(user, changePassword.OldPassword))
            {
                return await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            }
            return IdentityResult.Failed();
        }

        public string CheckAuth()
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return _contextAccessor.HttpContext.User.Identity.Name;
            }
            else return "not authenticated";
        }

        #endregion Authentication


        #region profile

        public UserDTO GetProfileInfo()
        {
            User user = this._context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name);
            Role userRole = this._context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            return new UserDTO { Id = user.Id, Email = user.Email, Name = user.Name, Surname = user.Surname, RoleId = user.RoleId, RoleName = userRole.Name };
        }

        public void UpdateProfileInfo(UserDTO userDTO)
        {
            User user = this._context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name);

            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;

            this._context.Users.Update(user);
            _context.Entry(user).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        #endregion profile

        #region Menus

        public List<MenuDTO> GetMenuListForWeek()
        {
            List<Menu> menus = this._context.Menus.Where(m=>m.Date.Date >= DateTime.Now.Date && m.Date.Date < DateTime.Now.Date.AddDays(8)).ToList();
            List<MenuDTO> menusDTO = new List<MenuDTO>();
            foreach(Menu menu in menus)
            {
                menusDTO.Add(new MenuDTO { Id = menu.Id, Date = menu.Date });
            }
            return menusDTO;
        }

        public List<MenuFoodDTO> GetMenuFoods(int menuId)
        {
            List<MenuFood> menuFoodList = this._context.MenuFoods.Where(mf => mf.MenuId == menuId).ToList();
            List<MenuFoodDTO> menuFoodDTOs = new List<MenuFoodDTO>();
            foreach(MenuFood mf in menuFoodList)
            {
                Food food = _context.Foods.FirstOrDefault(f => f.Id == mf.FoodId);
                Category category = _context.Categories.FirstOrDefault(c=>c.Id == food.CategoryId);
                menuFoodDTOs.Add(new MenuFoodDTO()
                {
                    Id = mf.Id,

                    FoodId = mf.FoodId,
                    MenuId = mf.MenuId,

                    Name = food.Name,
                    Weight = food.Weight,
                    Description = food.Description,
                    CategoryId = category.Id,
                    CategoryName = category.Name,

                    Price = mf.Price,
                    BaseAmount = mf.BaseAmount,
                    AvailableAmount = mf.AvailableAmount,
                    InsufficientAmount = mf.InsufficientAmount
                });
            }

            return menuFoodDTOs;
        }

        public List<MenuFoodDTO> GetMenuFoods()
        {
            List<MenuFood> menuFoodList = this._context.MenuFoods.Where(mf => mf.MenuId == this._context.Menus.FirstOrDefault(m=>m.Date == DateTime.Today.Date).Id).ToList();
            List<MenuFoodDTO> menuFoodDTOs = new List<MenuFoodDTO>();
            foreach (MenuFood mf in menuFoodList)
            {
                Food food = _context.Foods.FirstOrDefault(f => f.Id == mf.FoodId);
                Category category = _context.Categories.FirstOrDefault(c => c.Id == food.CategoryId);
                menuFoodDTOs.Add(new MenuFoodDTO()
                {
                    Id = mf.Id,

                    FoodId = mf.FoodId,
                    MenuId = mf.MenuId,

                    Name = food.Name,
                    Weight = food.Weight,
                    Description = food.Description,
                    CategoryId = category.Id,
                    CategoryName = category.Name,

                    Price = mf.Price,
                    BaseAmount = mf.BaseAmount,
                    AvailableAmount = mf.AvailableAmount,
                    InsufficientAmount = mf.InsufficientAmount
                });
            }

            return menuFoodDTOs;
        }

        //public Menu StupidGetMenus()
        //{
        //    Menu menu = new Menu();
        //    List<string> infoFields = new List<string>();

        //    Menu hehe = _context.Menus.FirstOrDefault(m => m.Id == 1);
        //    hehe.Info = "HAHAHAH";
        //    _context.Entry(hehe).State = EntityState.Modified;
        //    _context.SaveChanges();

        //    return hehe;

        //}

        #endregion Menus


        #region favourite

        private List<FoodDTO> ConvertFoodsToDTOs(List<Food> foodList)
        {
            List<FoodDTO> foodDTOList = new List<FoodDTO>();
            foreach (Food f in foodList)
            {
                Category category = _context.Categories.FirstOrDefault(c => c.Id == f.CategoryId);
                foodDTOList.Add(new FoodDTO { Id = f.Id, Name = f.Name, Description = f.Description, Weight = f.Weight, CategoryId = f.CategoryId, CategoryName = category.Name });
            }
            return foodDTOList;
        }

        public List<FoodDTO> GetFullFoodList()
        {
            List<Food> foodList = _context.ClientRepository.GetListOfFoods();
            return this.ConvertFoodsToDTOs(foodList);
        }

        public List<FoodDTO> GetFavouriteList()
        {
            List<Food> favouriteFoodList = _context.ClientRepository.GetListOfFavouriteFoodsForUserName(_contextAccessor.HttpContext.User.Identity.Name);
            return this.ConvertFoodsToDTOs(favouriteFoodList);
        }

        public void AddFavourite(int foodId)
        {
            User user = _context.Users.FirstOrDefault(u=>u.Email == _contextAccessor.HttpContext.User.Identity.Name);
            UserFavouriteFood newFavourite = new UserFavouriteFood() { FoodId = foodId, UserId = user.Id };
            if(_context.UserFavouriteFoods.FirstOrDefault(uff=>uff.FoodId == newFavourite.FoodId && uff.UserId == newFavourite.UserId) == null)
            {
                _context.UserFavouriteFoods.Add(newFavourite);
                _context.SaveChanges();
            }
        }

        public void RemoveFavourite(int foodId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name);
            UserFavouriteFood favouriteToRemove = _context.UserFavouriteFoods.FirstOrDefault(uff=>uff.FoodId == foodId && uff.UserId == user.Id);
            if (_context.UserFavouriteFoods.FirstOrDefault(uff => uff.FoodId == favouriteToRemove.FoodId && uff.UserId == favouriteToRemove.UserId) != null)
            {
                _context.UserFavouriteFoods.Remove(favouriteToRemove);
                _context.SaveChanges();
            }
        }

        #endregion favourite

        #region Reserve

        public void NotEnough(int menuFoodId)
        {
            MenuFood menuFood = _context.MenuFoods.FirstOrDefault(mf => mf.Id == menuFoodId);
            menuFood.InsufficientAmount++;
            _context.MenuFoods.Update(menuFood);
            _context.Entry(menuFood).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Reserve GetUserReserveInProcessing()
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name);
            Reserve reserve = _context.Reserves.FirstOrDefault(r => r.UserId == user.Id && r.IsAccepted == null);
            return reserve;
        }

        public bool IsUserHasReserveInProcessing()
        {
            if (GetUserReserveInProcessing() != null)
                return true;
            return false;
        }

        public double GetTimeLeftForExistingUserReserve()
        {
            return (DateTime.Now - GetUserReserveInProcessing().DateTime).TotalSeconds;
        }
        
        public void CreateReserve(List<CreateReserveModelDTO> reserve)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name);
            _context.ClientRepository.CreateReserveForUser(user, reserve);
        }

        public int GetUserReserve()
        {
            return GetUserReserveInProcessing().Id;
        }

        public void CheckReserves()
        {
            List<Reserve> reserves = _context.Reserves.Where(r=> r.IsAccepted == null).ToList();
            foreach(Reserve reserve in reserves)
            {
                if((DateTime.Now - reserve.DateTime).TotalSeconds > 600)
                {
                    this.DeclineReserve(reserve);
                }
            }

            _context.SaveChanges();
        }

        public void DeclineReserve(Reserve reserve)
        {
            reserve.IsAccepted = false;
            _context.Reserves.Update(reserve);
            _context.Entry(reserve).State = EntityState.Modified;

            List<MenuFoodReserve> menuFoodReserveList = _context.MenuFoodReserves.Where(mfr => mfr.ReserveId == reserve.Id).ToList();
            foreach(MenuFoodReserve mfr in menuFoodReserveList)
            {
                MenuFood mf = _context.MenuFoods.FirstOrDefault(mf => mf.Id == mfr.MenuFoodId);
                mf.AvailableAmount = mf.AvailableAmount + mfr.Amount;
                _context.MenuFoods.Update(mf);
                _context.Entry(mf).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        #endregion Reserve

        #region feedback

        public void WriteFeedback(FeedbackDTO feedbackDTO)
        {
            Feedback feedback;
            if (feedbackDTO.Anonymously)
            {
                feedback = new Feedback { Message = feedbackDTO.Message, UserId = null };
            }
            else
            {
                feedback = new Feedback { Message = feedbackDTO.Message, UserId = _context.Users.FirstOrDefault(u => u.Email == _contextAccessor.HttpContext.User.Identity.Name).Id };
            }

            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
        }

        #endregion feedback
    }
}
