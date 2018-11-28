using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using projetmvcfinale.Models;
using projetmvcfinale.Models.Authentification;

namespace projetmvcfinale.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        //propriétés du controlleur
        private readonly UserManager<LoginUser> _userManager;
        private readonly RoleManager<LoginRole> _roleManager;
        private readonly SignInManager<LoginUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IConfiguration Configuration;
        private readonly ProjetFrancaisContext provider;


        //constructeur
        public LoginController(
            UserManager<LoginUser> userManager,
            RoleManager<LoginRole> roleManager,
            SignInManager<LoginUser> signInManager,
            ILogger<LoginController> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl ?? "/Home/Index";
            return View();
        }


        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    ////ajoute le user dans une session
                    //this.HttpContext.Session.SetString("user", JsonConvert.SerializeObject(this.provider.selectclient(model.UserName)));
                    
                    _logger.LogInformation("User logged in.");

                   
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.Roles = this._roleManager.Roles.Select<LoginRole, SelectListItem>(
                r =>
                new SelectListItem()
                {
                    Text = r.Name,
                    Value = r.Name
                }
            ).ToList();

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel model, string retrunUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new LoginUser { UserName = model.Email, Email = model.Email, PhoneNumber=model.Telephone };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    this.provider.Utilisateur.Add(new Utilisateur { AdresseCourriel = model.Email, Nom = model.Nom, Prenom = model.Prenom, RegistrerDate = DateTime.Now });
                    this.provider.SaveChanges();
                    _logger.LogInformation("User created a new account with password.");
                    var roleresult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (roleresult.Succeeded)
                    {

                        _logger.LogInformation("User created a new account with password.");

                        return RedirectToAction("Home", "Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Erreur", roleresult.ToString());
                    }

                }
                else
                {
                    ModelState.AddModelError("Erreur", result.ToString());

                }

            }

            // If we got this far, something failed, redisplay form
            model.Roles = this._roleManager.Roles.Select<LoginRole, SelectListItem>(
                r =>
                new SelectListItem()
                {
                    Text = r.Name,
                    Value = r.Name
                }
            ).ToList();
            return View(model);
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["Url"] = returnUrl;
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Accueil", "Librairie");
        }
      


    }
}