using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BuffetClientMVC.Services;
using BuffetClientMVC.Models;
using BuffetAuxiliaryLib.DTOs;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace BuffetClientMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly WebApiMessagingHandler _apiHandler;
        private readonly ClientService _clientService;

        public ClientController(WebApiMessagingHandler webApiHandler, ClientService clientService)
        {
            _apiHandler = webApiHandler;
            _clientService = clientService;
        }

        public IActionResult Index()
        {
            _apiHandler.CheckReservesTimeExpiration();
            return View();
        }

        #region Favourites

        [HttpGet]
        [Authorize]
        public IActionResult ShowFavourites()
        {
            if(_apiHandler.GetFavouriteList() != null)
            {
                return View(this._apiHandler.GetFavouriteList().Result);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowFullFoodList()
        {
            if(_apiHandler.GetFavouriteList() != null && _apiHandler.GetFullFoodList() != null)
            {
                ViewBag.FavouriteList = this._apiHandler.GetFavouriteList().Result;
                ViewBag.FoodList = this._apiHandler.GetFullFoodList().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddFoodToFavourites(int id)
        {
            await this._apiHandler.AddFoodToFavourites(id);
            return RedirectToAction("ShowFullFoodList");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveFoodFromFavourites(int id) 
        {
            await this._apiHandler.RemoveFoodFromFavourites(id); 
            return RedirectToAction("ShowFullFoodList");
        }

        #endregion Favourites

        #region Menu

        [HttpGet]
        public IActionResult ShowMenus()
        {
            try
            {
                ViewBag.Menus = this._apiHandler.ReadMenusForWeek().Result;
                return View();
            }
            catch (Exception e)
            {
                Log.Error(e, "An exception was caught");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult ReadMenu(int id) 
        {                                                   
            try
            {
                ViewBag.Menu = this._apiHandler.ReadMenu(id).Result;
                return View();
            }
            catch (Exception e)
            {
                Log.Error(e, "An exception was caught");
                return RedirectToAction("Index");
            }                               
        }

        #endregion Menu

        #region Reserve

        [HttpGet]
        [Authorize]
        public IActionResult CreateReserve()  
        {
            try
            {
                ViewBag.Menu = this._apiHandler.ReadMenu().Result;
                return View();
            }
            catch (Exception e)
            {
                Log.Error(e, "An exception was caught");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReserve(int[] ids, int[] quantity, int[] available)  // принимает модель
        {
            ViewBag.Menu = this._apiHandler.ReadMenu().Result;

            if (this._apiHandler.IsUserHasReserveInProcessing().Result) /// Здесь для iva остановиться должен был
            {
                ViewBag.Message = "You already have a reserve, come to take it, you have " + _apiHandler.GetTimeLeftForExistingUserReserve().Result + " seconds left.";
                return View(); //return show existing
            }

            if(quantity.Sum() == 0 ) //hehe
            {
                ViewBag.Message = "You must type numbers into right column to create reserve";
                return View();
            }

            if (_clientService.IsUserAddsMoreThanAvailable(quantity, available))
            {
                ViewBag.Message = "You have chosen amount that is more than available.";
                return View();
            }

            var response = await _apiHandler.CreateReserve(ids, quantity);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ShowCurrentUserReserve"); //reserves ? 
            }

            ViewBag.Message = "Failed to create reserve, try again";
            return View();
        }

        //[HttpGet]
        //[Authorize]
        //public IActionResult ShowUserReserves() 
        //{                                                
        //    return View();
        //}

        [HttpGet]
        [Authorize]
        public IActionResult ShowCurrentUserReserve()
        {
            ViewBag.Code = _apiHandler.GetUserReserveInProcessing(); //выбросить сюда код итд
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> NotEnough(int id)
        {
            await _apiHandler.NotEnough(id);
            return RedirectToAction("CreateReserve");
        }

        #endregion Reserve

        #region Authentication

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) 
        {
            if (ModelState.IsValid) 
            {
                RegisterDTO registerInfoDTO = new RegisterDTO {
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Password = model.Password
                };

                var response = await this._apiHandler.Register(registerInfoDTO);

                if (response != null && response.IsSuccessStatusCode) // &&
                {
                    await _clientService.CreateAuthCookieWithTokenForUser(await response.Content.ReadAsStringAsync(), registerInfoDTO.Email);
                    return RedirectToAction("Index");
                }
                else
                {
                    Log.Information("Unsuccessful status code was recieved from web api response. Code: " + response.StatusCode);
                }

            }
            else
            {
                Log.Information("Recieved Login model is not valid.");
                ModelState.AddModelError("", "Not valid");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                LoginDTO loginInfoDTO = new LoginDTO { Email = model.Email, Password = model.Password };

                var response = await this._apiHandler.Login(loginInfoDTO);

                if (response != null && response.IsSuccessStatusCode)
                {
                    await _clientService.CreateAuthCookieWithTokenForUser(await response.Content.ReadAsStringAsync(), loginInfoDTO.Email);
                    return RedirectToAction("Index");
                }
                else
                {
                    Log.Information("Unsuccessful status code was recieved from web api response. Code: " + response.StatusCode);
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                Log.Information("Recieved Login model is not valid.");
                ModelState.AddModelError("", "Input data was not valid");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var response = await _apiHandler.Logout();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                ChangePasswordDTO changePasswordDTO = _clientService.GetChangePasswordDTO(changePasswordModel);
                var response = await this._apiHandler.ChangePassword(changePasswordDTO);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
            }
            return View(changePasswordModel);
        }


        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

        #endregion Authentication

        #region profile

        [HttpGet]
        [Authorize]
        public IActionResult ShowUserProfile()
        {
            return View(_apiHandler.GetUserProfileInfo().Result);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangeUserProfile()
        {

            return View(_apiHandler.GetUserProfileInfo().Result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeUserProfile(UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var response = await this._apiHandler.UpdateUserProfileInfo(user);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ShowUserProfile");
                }
            }
            else
            {
                ModelState.AddModelError("", "Model is not valid");
            }
            return View(user);
        }

        #endregion profile

        #region feedback

        [HttpGet]
        [Authorize]
        public IActionResult WriteFeedback()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult WriteFeedback(FeedbackViewModel feedback)
        {
            try
            {
                var response = _apiHandler.SendFeedback(feedback);
                if (response.Result.IsSuccessStatusCode)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        #endregion feedback
    }
}
