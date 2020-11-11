using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BuffetClientMVC.Extensions
{
    public static class HttpClientExtensions
    {
        private static IHttpContextAccessor _contextAccessor = new HttpContextAccessor(); //тупизна

        public static void SetTokenAuthorizeHeader(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token").Value);
        }
    }
}
