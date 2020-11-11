using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuffetClientMVC.Services;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace BuffetClientMVC
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string baseWebApiUrl = Configuration.GetSection("WebApiURL").Value;
            IHttpContextAccessor contextAccessor = new HttpContextAccessor();//тупизна

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = ".AspNetCore.Cookie";
                });

            services.AddHttpClient<WebApiMessagingHandler>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(180);
                client.BaseAddress = new Uri(baseWebApiUrl);
                if(contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token") != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token").Value);
                }
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ClientService>();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 44300;
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();

            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Client}/{action=Index}/{id?}");
            });
        }
    }
}
