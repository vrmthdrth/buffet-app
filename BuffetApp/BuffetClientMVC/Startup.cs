using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuffetClientMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using BuffetClientMVC.Hubs;

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
            IHttpContextAccessor contextAccessor = new HttpContextAccessor();

            services.AddHttpContextAccessor();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = ".AspNetCore.Cookie";
                });

            services.AddHttpClient<WebApiMessagingHandler>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(180);
                client.BaseAddress = new Uri(baseWebApiUrl);
                if (contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token") != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token").Value);
                }
            });

            services.AddSingleton<ClientService>();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 44300;
            });

            services.AddSignalR();
            services.AddRazorPages();

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
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
