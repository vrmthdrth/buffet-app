using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BuffetAuxiliaryLib.DTOs;
using BuffetDAL.Models;
using BuffetWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BuffetWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApiService _apiService;

        public ClientController(ApiService apiService)
        {
            this._apiService = apiService;
        }

        #region Menus

        [HttpGet]
        [Route("readmenusforweek")]
        public IActionResult GetMenuListForWeek()
        {
            try
            {
                return Ok(this._apiService.GetMenuListForWeek());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("readmenu")]
        public IActionResult ReadMenu([FromQuery]int id)
        {
            try
            {
                _apiService.CheckReserves();
                return Ok(this._apiService.GetMenuFoods(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("readmenufortoday")]
        public IActionResult ReadMenu()
        {
            try
            {
                return Ok(this._apiService.GetMenuFoods());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion Menus

        #region profile

        [HttpGet]
        [Authorize]
        [Route("getprofileinfo")]
        public IActionResult GetProfileInfo()
        {
            try
            {
                return Ok(this._apiService.GetProfileInfo());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("updateprofileinfo")]
        public IActionResult UpdateProfileInfo([FromBody] UserDTO userDTO)
        {
            try
            {
                this._apiService.UpdateProfileInfo(userDTO);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion profile

        #region favourite

        [HttpGet]
        [Authorize]
        [Route("getfullfoodlist")]
        public IActionResult GetFullFoodList()
        {
            try
            {
                return Ok(this._apiService.GetFullFoodList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getfavouritelist")]
        public IActionResult GetFavouriteList()
        {
            try
            {
                return Ok(this._apiService.GetFavouriteList());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("addfavourite")]
        public IActionResult AddFavourite([FromQuery] int id) //foodid
        {
            try
            {
                this._apiService.AddFavourite(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("removefavourite")]
        public IActionResult RemoveFavourite([FromQuery] int id)
        {
            try
            {
                this._apiService.RemoveFavourite(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion favourite

        #region Reserve

        [HttpGet]
        [Authorize]
        [Route("notenough")]
        public IActionResult NotEnough(int id)
        {
            try
            {
                this._apiService.NotEnough(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("isuserhasreserve")]
        public IActionResult IsUserHasReserveInProcessing()
        {
            try
            {
                if (_apiService.IsUserHasReserveInProcessing())
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Authorize]
        [Route("gettimeleft")]
        public IActionResult GetTimeLeftForExistingUserReserve()
        {
            try
            {
                return Ok(_apiService.GetTimeLeftForExistingUserReserve());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("createreserve")]
        public IActionResult CreateReserve([FromBody] List<CreateReserveModelDTO> reserve)
        {
            try
            {
                _apiService.CheckReserves();
                _apiService.CreateReserve(reserve);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getuserreserve")]
        public IActionResult GetUserReserve()
        {
            try
            {
                return Ok(_apiService.GetUserReserve()); //поправить
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("checkreserves")]
        public IActionResult CheckReserves()
        {
            try
            {
                _apiService.CheckReserves();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion Reserve


        #region feedback

        [HttpPost]
        [Authorize]
        [Route("writefeedback")]
        public IActionResult WriteFeedback([FromBody]FeedbackDTO feedbackDTO)
        {
            try
            {
                _apiService.WriteFeedback(feedbackDTO);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion feedback
    }
}
