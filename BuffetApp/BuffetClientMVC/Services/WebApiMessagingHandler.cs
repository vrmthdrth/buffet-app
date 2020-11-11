using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BuffetAuxiliaryLib.DTOs;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

using BuffetClientMVC.Extensions;
using System.Net;
using BuffetClientMVC.Models;
using System.Reflection.Metadata.Ecma335;

namespace BuffetClientMVC.Services
{
    public class WebApiMessagingHandler
    {
        private readonly HttpClient _httpClient;

        private IHttpContextAccessor _contextAccessor;

        public Uri BaseAddress => this._httpClient.BaseAddress;

        public WebApiMessagingHandler(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            this._httpClient = httpClient;
            this._contextAccessor = contextAccessor;
        }

        #region Favourites

        public async Task<List<FoodDTO>> GetFullFoodList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/getfullfoodlist");
            if (response.IsSuccessStatusCode)
            {
                List<FoodDTO> result = JsonConvert.DeserializeObject<List<FoodDTO>>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            return null;
        }

        public async Task<List<FoodDTO>> GetFavouriteList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/getfavouritelist");
            if (response.IsSuccessStatusCode)
            {
                List<FoodDTO> result = JsonConvert.DeserializeObject<List<FoodDTO>>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            return null;
        }

        public async Task<bool> AddFoodToFavourites(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/addfavourite?id={id}"); // ..?id={id}
            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<bool> RemoveFoodFromFavourites(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/removefavourite?id={id}");
            return response.IsSuccessStatusCode ? true : false;
        }

        #endregion Favourites

        #region Menu

        public async Task<List<MenuDTO>> ReadMenusForWeek()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/readmenusforweek");
            if (response.IsSuccessStatusCode)
            {
                List<MenuDTO> result = JsonConvert.DeserializeObject<List<MenuDTO>>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            return null; 
        }

        public async Task<List<MenuFoodDTO>> ReadMenu(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/readmenu?id={id}");
            if (response.IsSuccessStatusCode)
            {
                List<MenuFoodDTO> result = JsonConvert.DeserializeObject<List<MenuFoodDTO>>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            return null;
        }

        public async Task<List<MenuFoodDTO>> ReadMenu()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/readmenufortoday");
            if (response.IsSuccessStatusCode)
            {
                List<MenuFoodDTO> result = JsonConvert.DeserializeObject<List<MenuFoodDTO>>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            return null;
        }

        public async Task<List<MenuFoodDTO>> ReadMenuListForMenuId(int menuId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/client/getmenulist/{menuId}");
                if (response.IsSuccessStatusCode)
                {
                    List<MenuFoodDTO> listOfMenuFoods = JsonConvert.DeserializeObject<List<MenuFoodDTO>>(response.Content.ReadAsStringAsync().Result);
                    return listOfMenuFoods;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion Menu

        #region Authentication

        public async Task<HttpResponseMessage> Login(LoginDTO loginInfoDTO)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(loginInfoDTO), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"api/auth/login", content);
                return response;
            }
            catch (Exception e)
            {
                Log.Warning(e, "Failed to send message or get response");
                return null;
            }
        }

        public async Task<HttpResponseMessage> Register(RegisterDTO registerInfoDTO)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(registerInfoDTO), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"api/auth/register", content);
                return response;
            }
            catch (Exception e)
            {
                Log.Warning(e, "Failed to send message or get response");
                return null;
            }
        }

        public async Task<HttpResponseMessage> Logout()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/auth/logout");
            if (response.IsSuccessStatusCode)
            {
                await _contextAccessor.HttpContext.SignOutAsync(); // delete cookies on client mvc site
            }
            return response;
        }

        public async Task<UserDTO> GetUserProfileInfo()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/getprofileinfo");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<UserDTO>(response.Content.ReadAsStringAsync().Result);
            }
            return null;
        }

        public async Task<HttpResponseMessage> UpdateUserProfileInfo(UserDTO updatedUserDTO)
        {
            var content = new StringContent(JsonConvert.SerializeObject(updatedUserDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"api/client/updateprofileinfo", content);
            return response;
        }

        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var content = new StringContent(JsonConvert.SerializeObject(changePasswordDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"api/auth/changePassword", content);
            return response;
        }

        #endregion Authentication

        #region Reserves

        public async Task<HttpResponseMessage> NotEnough(int menuFoodId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/notenough?id={menuFoodId}");
            return response;
        }

        public async Task<bool> IsUserHasReserveInProcessing()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/isuserhasreserve");
            var result = await response.Content.ReadAsStringAsync();
            bool isUserHasReserve;
            Boolean.TryParse(result, out isUserHasReserve);
            if(isUserHasReserve)
            {
                return true;
            }

            return false;
        }

        public async Task<double> GetTimeLeftForExistingUserReserve()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/client/gettimeleft");
            double timeSecondsLeft;
            Double.TryParse(response.Content.ReadAsStringAsync().Result, out timeSecondsLeft);
            return timeSecondsLeft;
        }

        public async Task<HttpResponseMessage> CreateReserve(int[] menuFoodIds, int[] quantities)
        {
            List<CreateReserveModelDTO> reserveModel = new List<CreateReserveModelDTO>();

            for(int i = 0; i < quantities.Length; i++)
            {
                if(quantities[i] != 0)
                {
                    reserveModel.Add(new CreateReserveModelDTO { Id = menuFoodIds[i], Quantity = quantities[i] });
                }
            }

            var content = new StringContent(JsonConvert.SerializeObject(reserveModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("api/client/createreserve", content);
            return response;
        }

        public async Task<double> GetUserReserveInProcessing()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/client/getuserreserve");
            double code;
            double.TryParse(await response.Content.ReadAsStringAsync(), out code);
            return code;
        }

        public void CheckReservesTimeExpiration()
        {
            _httpClient.GetAsync("api/client/checkreserves");
        }

        #endregion Reserves

        #region feedback

        public async Task<HttpResponseMessage> SendFeedback(FeedbackViewModel feedback)
        {
            FeedbackDTO feedbackDTO = new FeedbackDTO { Message = feedback.Message, Anonymously = feedback.Anonymously };
            var content = new StringContent(JsonConvert.SerializeObject(feedbackDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"api/client/writefeedback", content);
            return response;
        }

        #endregion feedback

    }
}
