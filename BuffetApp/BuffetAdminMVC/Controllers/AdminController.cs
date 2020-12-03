using System;
using System.Collections.Generic;
using System.Data;
using BuffetAdminMVC.Models;
using BuffetDAL.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using BuffetDAL.AdditionalModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BuffetAdminMVC.Services;
using Microsoft.AspNetCore.Identity;

namespace BuffetAdminMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            Log.Information("Admin/Index GET method execution.");
            return View();
        }

        #region Admins
        [Authorize(Roles = "SA")]
        [HttpGet]
        public IActionResult ViewAdmins()
        {
            try
            {
                Log.Information("Admin/ViewAdmins GET method execution started");
                List<User> admins = _adminService.GetAdmins();
                ViewBag.Admins = admins;
                return View();
            }
            catch(Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/ViewAdmins GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "SA")]
        [HttpGet]
        public IActionResult AddAdmin()
        {
            Log.Information("Admin/AddAdmin GET method execution");
            return View();
        }

        [Authorize(Roles = "SA")]
        [HttpPost]
        public async Task<IActionResult> AddAdmin(RegisterViewModel adminModel)
        {
            Log.Information("Admin/AddAdmin POST method execution started");
            try
            {
                if (ModelState.IsValid)
                {
                    if (_adminService.IsEmailNotExist(adminModel.Email))
                    {
                        var result = _adminService.CreateIdentityUser(adminModel).Result;
                        if (result.Succeeded)
                        {
                            await _adminService.AddIdentityUserToRole(adminModel);  //внутри находит созданного на основе adminmodel IdentityUser и устанавливает ему роль
                            _adminService.CreateUserAndAddToDb(adminModel);         //добавляет юзера в Users table
                            return RedirectToAction("ViewAdmins");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email already exists");
                    }
                }
                return View(adminModel);
            }
            catch(Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/AddAdmin POST method execution. Probably database access exception. ");
                return RedirectToAction("Index");            
            }
        }

        [Authorize(Roles = "SA")]
        [HttpGet]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                Log.Information("Admin/DeleteAdmin GET method execution started");
                await this._adminService.DeleteAdmin(id);
                return RedirectToAction("ViewAdmins");
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/DeleteAdmin GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        #endregion Admins

        #region Foods

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult ShowFoods()
        {
            Log.Information("Admin/ShowFoods GET method execution started");
            try
            {
                ViewBag.Categories = _adminService.GetAllCategories();
                ViewBag.Foods = _adminService.GetAllFoods();
                return View();
            }
            catch (Exception e)
            {
                Log.Warning("An exception was caught during Admin/ShowFoods GET method execution. Probably database access exception. ", e.Message);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult CreateFood()
        {
            Log.Information("Admin/CreateFood GET method execution started");
            try
            {
                ViewBag.Categories = _adminService.GetAllCategories();
                return View();
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/CreateFood GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "SA, Admin")]
        [HttpPost]
        public IActionResult CreateFood(Food food, string category)
        {
            Log.Information("Admin/CreateFood POST method execution started");
            try
            {
                if (ModelState.IsValid)
                {
                    List<Food> foodsList = _adminService.GetAllFoods();
                    _adminService.SetCategoryForFood(food, category);
                    if (!_adminService.IsFoodUnique(food))
                    {
                        ViewBag.Message = "This food is not unique by name.";
                        ViewBag.Categories = _adminService.GetAllCategories();
                        return View(food);
                    }

                    _adminService.AddFoodToDb(food);

                    return RedirectToAction("ShowFoods");
                }

                return View(food);
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/CreateFood POST method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        #endregion Foods

        #region Menus

        [HttpGet]
        public IActionResult ShowMenus()
        {
            Log.Information("Admin/ShowMenus GET method execution started"/*at threadId: {Thread.CurrentThread.ManagedThreadId}*/);
            try
            {
                ViewBag.Menus = _adminService.GetAllMenus();
                return View();
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/ShowMenus GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult ShowMenuDetails(int id)
        {
            Log.Information("Admin/ShowMenuDetails GET method execution started");
            try
            {
                _adminService.CheckReserves();
                DataTable menuDetails = _adminService.ReadMenuForDayAsDataTable(id);
                if(menuDetails.Rows.Count == 0)
                {
                    Log.Information("A Database request result is empty. Probably there's no menu or menulist for recieved menu id: " + id);
                }
                ViewBag.MenuDate = _adminService.ReadMenuById(id).Date;
                return View(menuDetails);
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/ShowMenuDetails GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult CreateMenuForDate()
        {
            try
            {
                Log.Information("Admin/CreateMenuForDate GET method execution started");
                ViewBag.Foods = _adminService.GetAllFoods();
                return View();
            }
            catch(Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/CreateMenuForDate GET method execution. Probably database access exception. ");
                return RedirectToAction("ShowMenus");
            }
        }

        [HttpPost]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult CreateMenuForDate(string[] foods, string[] price, string[] baseAmount, string menuDate) 
        {
            try
            {
                Log.Information("Admin/CreateMenuForDate POST method execution started");
                ViewBag.Foods = _adminService.GetAllFoods();
                ViewBag.Categories = _adminService.GetAllCategories();

                if (String.IsNullOrEmpty(menuDate) || _adminService.IsArrayEmpty(foods) || _adminService.IsArrayEmpty(price) || _adminService.IsArrayEmpty(baseAmount) ) //IsArrayEmpty
                {
                    Log.Information("Admin/CreateMenuForDate POST method: Length of array of values which were submitted is 0.");
                    return View();
                } 

                DateTime menuDateTime = _adminService.ConvertFromStringToDateTime(menuDate);

                if (_adminService.IsDateNotInValidRange(menuDateTime))
                {
                    ViewBag.Message = "Date must be in range which was displayed in alert message";
                    return View();
                }

                if (_adminService.ReadMenuForDate(menuDateTime).Date.ToString("yyyy-MM-dd") != menuDate)
                {

                    List<MenuRowModel> menuRows = _adminService.FillMenuRows(foods, price, baseAmount);

                    if (_adminService.IsPriceOrBaseAmountNotValid(menuRows))
                    {
                        ViewBag.Message = $"Price can not be negative. Base amount can not be less than 0.";
                        return View();
                    }

                    if (_adminService.IsThereDuplicatesInMenuRows(menuRows))
                    {
                        ViewBag.Message = "There are duplicates in your menu. Try again.";
                        return View();
                    }

                    _adminService.CreateMenuWithList(menuDateTime, menuRows);

                    return RedirectToAction("ShowMenus");
                }
                else
                {
                    ViewBag.Message = "The menu for chosen date is already exist. Choose another date.";
                    ViewBag.Foods = _adminService.GetAllFoods();
                    return View();
                }
            }
            catch(Exception e)
            {
                Log.Warning(e, "An exception was caught in attempt to create menu.");
                ViewBag.Foods = _adminService.GetAllFoods();
                return View();
            } 
        }

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult UpdateMenuFoodsAmountForTommorrow(int id)     // выносить логику в сервис
        {
            try
            {
                Log.Information("Admin/UpdateMenuFoodsAmountForTommorrow GET method execution started");
                ViewBag.MenuDetails = _adminService.ReadMenuForDayAsList(id);
                ViewBag.MenuId = id; 
                return View();
            }
            catch(Exception e)
            {
                Log.Warning(e, "An exception was caught in attempt to update menu.");
                ViewBag.Foods = _adminService.GetAllFoods();
                return RedirectToAction("ShowMenus");
            }
        }

        [Authorize(Roles = "SA, Admin")]
        [HttpPost]
        public IActionResult UpdateMenuFoodsAmountForTommorrow(string[] id, string[] baseAmount, string menuId)
        {
            try
            {
                Log.Information("Admin/UpdateMenuFoodsAmountForTommorrow POST method execution started");

                _adminService.UpdateMenuFoodsAmountForTomorrow(id, baseAmount, menuId);

                return RedirectToAction("ShowMenus");
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught in attempt to update menu.");
                ViewBag.Foods = _adminService.GetAllFoods();
                return RedirectToAction("ShowMenus");
            }
        }

        #endregion Menus

        #region Statistics

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult ShowStatistics(string reportType)
        {
            Log.Information("Admin/ShowStatistics GET method execution started");
            try
            {
                string reportMessage;
                DataTable report = _adminService.CreateReportTable(reportType, out reportMessage);
                ViewBag.ReportName = reportMessage;
                return View(report);
            }
            catch (Exception e)
            {
                Log.Warning(e, "An exception was caught during Admin/ShowStatistics GET method execution. Probably database access exception. ");
                return RedirectToAction("Index");
            }
        }

        #endregion Statistics

        #region Feedbacks

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult ShowFeedbacks()
        {
            try
            {
                Log.Information("Admin/ShowFeedbacks GET method execution started");
                ViewBag.Feedbacks = _adminService.GetAllFeedbacks();
                return View();
            }
            catch(Exception e)
            {
                Log.Warning(e, "Failed to execute ShowFeedbacks() GET method");
                return RedirectToAction("Index");
            }
        }

        #endregion Feedbacks

        #region Reserves
        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult ShowReserves()
        {
            try
            {
                _adminService.CheckReserves();
                ViewBag.Reserves = _adminService.GetReservesWithUserNames();
                return View();
            }
            catch(Exception e)
            {
                Log.Warning(e, "Failed to execute ShowReserves() GET method.");
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "SA, Admin")]
        [HttpGet]
        public IActionResult ShowReserveDetails(int id)
        {
            try
            {
                Log.Information("Admin/ShowReserveDetails GET method execution started");
                ViewBag.Reserve = _adminService.GetReserveWithUserName(id); 
                return View(_adminService.GetReserveDetails(id));
            }
            catch(Exception e)
            {
                Log.Warning(e, "Failed to read reserve details for reserve id = " + id);
                return RedirectToAction("ShowReserves");
            }
        }

        [HttpGet]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult AcceptReserve(int id)
        {
            try
            {
                Log.Information("Admin/AcceptReserve GET method execution started");
                if (!_adminService.IsTimeForReserveExpired(id)) 
                {
                    _adminService.AcceptReserve(id);
                }
                else
                {
                    _adminService.DeclineReserve(id);
                }
            }
            catch(Exception e)
            {
                Log.Warning(e, "Failed to accept the reserve (Admin/AcceptReserve) id = " + id);
            }

            return RedirectToAction("ShowReserves");
        }

        #endregion Reserves

        #region Authentication

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            Log.Information("Admin/Login GET method execution started");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                Log.Information("Admin/Login POST method execution started");
                if (ModelState.IsValid)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _adminService.ValidateAndAuthenticate(model.Email, model.Password);

                    if (result.Succeeded && _adminService.IsUserInSAAdminRole(model.Email))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect login or password.");
                    }
                }
            }
            catch(Exception e)
            {
                Log.Warning(e, "Failed to login (Admin/Login POST)");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _adminService.SignOutAsync();
            return RedirectToAction("Index", "Admin");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Forbidden()
        {
            Log.Information("Admin/Forbidden GET method execution started");
            return View();
        }

        #endregion Authentication

        #region Profile

        [HttpGet]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult ShowUserProfile()
        {
            Log.Information("Admin/ShowUserProfile GET method execution started");
            return View(_adminService.GetUserProfileInfo());
        }

        [HttpGet]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult ChangeProfileInfo()
        {
            Log.Information("Admin/ChangeProfileInfo GET method execution started");
            return View(_adminService.GetUserProfileInfo());
        }

        [HttpPost]
        [Authorize(Roles = "SA, Admin")]
        public IActionResult ChangeProfileInfo(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Log.Information("Admin/ChangeProfileInfo GET method execution started");
                    _adminService.UpdateUserProfileInfo(user);
                }
                else
                {
                    return View(user);
                }
            }
            catch(Exception e)
            {
                Log.Error(e, "Failed to update user profile info.");
            }

            return RedirectToAction("ShowUserProfile");
        }

        #endregion Profile

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public string CheckAjax(string text)
        {
            return text + "hahahah";
        }
    }
}
