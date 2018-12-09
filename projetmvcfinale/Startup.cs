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
        private ProjetFrancaisContext contexteActu;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.contexteActu = new ProjetFrancaisContext(configuration.GetConnectionString("DefaultConnection"));
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
                options.CheckConsentNeeded = context => false;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseSession();
            app.UseMvc();
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

            CreateRoles(serviceProvider).Wait();
        }

        //https://www.c-sharpcorner.com/article/role-base-authorization-in-asp-net-core-2-1/
        //creation des roles et des utilisateurs au demarrage
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles   

            var RoleManager = serviceProvider.GetRequiredService<RoleManager<LoginRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<LoginUser>>();
            string[] roleNames = { "Admin", "Usager"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1  
                    roleResult = await RoleManager.CreateAsync(new LoginRole() { Name = roleName, });
                }
            }

            LoginUser user = await UserManager.FindByEmailAsync("Francais@gmail.com");

            if (user == null)
            {
                user = new LoginUser()
                {
                    UserName = "Francais@gmail.com",
                    Email = "Francais@gmail.com",
                };
                var CreateUser = await UserManager.CreateAsync(user, "password1");
                if (CreateUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, "Admin");
                    Utilisateur util = new Utilisateur() { Nom = "Francais@gmail.com", Prenom = "Francais@gmail.com", RegistrerDate = DateTime.Now, AdresseCourriel = "Francais@gmail.com" };
                    contexteActu.Utilisateur.Add(util);
                    await contexteActu.SaveChangesAsync();
                }
            }

            LoginUser user1 = await UserManager.FindByEmailAsync("Usager@gmail.com");

            if (user1 == null)
            {
                user1 = new LoginUser()
                {
                    UserName = "Usager@gmail.com",
                    Email = "Usager@gmail.com",
                };
                var CreateUser = await UserManager.CreateAsync(user1, "bullshit1234");
                if (CreateUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user1, "Usager");
                    Utilisateur util = new Utilisateur() { Nom = "Usager@gmail.com", Prenom = "Usager@gmail.com", RegistrerDate = DateTime.Now, AdresseCourriel = "Usager@gmail.com" };
                    contexteActu.Utilisateur.Add(util);
                    await contexteActu.SaveChangesAsync();
                }

            }

        }
    }

}
