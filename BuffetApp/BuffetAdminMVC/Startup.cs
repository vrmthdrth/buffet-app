using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuffetAdminMVC.Services;
using BuffetDAL.Repos.ADO;
using BuffetDAL.Repos.EF;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace BuffetAdminMVC
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
            string connection = Configuration.GetConnectionString("Default");
            services.AddScoped<ADOUnitOfWork>(service => new ADOUnitOfWork(connection));
            services.AddDbContext<IdentityEFContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityEFContext>();
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                options =>
                {
                    options.LoginPath = "/Admin/Login";
                    options.AccessDeniedPath = "/Admin/Forbidden";
                    //options.ExpireTimeSpan = new TimeSpan(0, 1, 0);
                });

            services.AddScoped<AdminService>();

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
                app.UseExceptionHandler("/Admin/Error");
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
                    pattern: "{controller=Admin}/{action=Index}/{id?}");
            });
        }
    }
}