using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using BuffetAuxiliaryLib.DTOs;
using BuffetWebAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BuffetWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            this._apiService = apiService;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] RegisterDTO registerInfoDTO)  
        {
            try
            {
                Log.Information("Auth/Register GET method execution started");
                if(_apiService.IsEmailNotExist(registerInfoDTO.Email))
                {
                    var result = _apiService.AddNewUserToDataBase(registerInfoDTO);
                    if (result.Result.Succeeded)
                    {
                        return Ok(_apiService.GenerateJwtToken(registerInfoDTO.Email));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Conflict("Entered E-mail address is already exists");
                }

            }
            catch(Exception e)
            {
                Log.Warning(e, "An exeption was caught during Auth/Register GET method execution");

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginInfoDTO) 
        {
            try
            {
                Log.Information("Auth/Login GET method execution started");
                var result = await _apiService.ValidateCredentialsForLogin(loginInfoDTO); //log info user credentials didnt pass validation
                if (result.Succeeded && _apiService.IsUserInUserRole(loginInfoDTO.Email))
                {
                    return Ok(_apiService.GenerateJwtToken(loginInfoDTO.Email)); 
                }
                else
                {
                    return BadRequest("Wrong email or password.");
                }
            }
            catch(Exception e)
            {
                Log.Warning(e, "Something gone completely wrong");
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    await _apiService.Logout();
                }

                if (HttpContext.User.Identity.IsAuthenticated == false)
                {
                    Log.Information("User cookie were deleted --- ");
                }

                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var result = await _apiService.ValidateAndChangePassword(changePasswordDTO);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Maybe have entered wrong old password. \nNew password must contain:\nlatin letters in upper and lower case (a..z, A..Z), \nat least one number (0..9), \nat least one special symbol(!, @, #, etc)");
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("CheckAuth")]
        public string CheckAuth()
        {
            return _apiService.CheckAuth();
        }

    }
}
