using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BuffetAuxiliaryLib.DTOs;
using BuffetClientMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace BuffetClientMVC.Services
{
    public class ClientService
    {
        private IHttpContextAccessor _contextAccessor;

        public ClientService(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }

        public async Task CreateAuthCookieWithTokenForUser(string token, string userEmail)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userEmail),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "User"),
                    new Claim("token", token)
                };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        public ChangePasswordDTO GetChangePasswordDTO(ChangePasswordViewModel model)
        {
            return  new ChangePasswordDTO {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword,
                PasswordConfirm = model.PasswordConfirm
            };
        }

        public List<CreateReserveModelDTO> CreateReserveInfoModel(int[] ids, int[] quantity)
        {
            List<CreateReserveModelDTO> reserveInfo = new List<CreateReserveModelDTO>();
            for(int i = 0; i < quantity.Length; i++)
            {
                if(quantity[i] != 0)
                {
                    reserveInfo.Add(new CreateReserveModelDTO { Id = ids[i], Quantity = quantity[i] });
                }
            }
            return reserveInfo;
        }

        public bool IsUserAddsMoreThanAvailable(int[] quantity, int[] available)
        {
            for(int i = 0; i < quantity.Length; i ++)
            {
                if(available[i] - quantity[i] < 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
