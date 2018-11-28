using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using projetmvcfinale.Models;
using projetmvcfinale.Models.Authentification;

namespace projetmvcfinale
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          



            //ajouter le service d'authentification Identity
            services.AddIdentity<LoginUser, LoginRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = false;

            }).AddEntityFrameworkStores<LoginDbContext>()
                .AddDefaultTokenProviders();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Login";
                options.ReturnUrlParameter = "ReturnUrl";
                options.LogoutPath = "/Login/Logout";
                options.AccessDeniedPath = "/Login/AccessDenied";
                options.ExpireTimeSpan = new TimeSpan(0, 15, 0);

            });

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //ajouter le service EntityFramework
            services.AddDbContext<LoginDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LoginConnection")));

            services.AddDbContext<ProjetFrancaisContext>(options =>
               options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
