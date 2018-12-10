using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using projetmvcfinale.Models;
using projetmvcfinale.Models.Authentification;

namespace projetmvcfinale.Controllers
{
    public class UtilisateurController : Controller
    {


        //propriétés du controlleur
        private readonly UserManager<LoginUser> _userManager;
        private readonly RoleManager<LoginRole> _roleManager;
        private readonly LoginDbContext providerlogin;
        private readonly ILogger _logger;
        private readonly IConfiguration Configuration;
        private readonly ProjetFrancaisContext provider;


        //constructeur
        public UtilisateurController(
            UserManager<LoginUser> userManager,
            RoleManager<LoginRole> roleManager,
            ILogger<LoginController> logger, IConfiguration configuration, LoginDbContext log)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            this.Configuration = configuration;
            this.providerlogin =log;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }
        public IActionResult ListeUtilisateur()
        {

            List<Utilisateur> listeUser = this.provider.Utilisateur.ToList();

            //List<UtilisateurPersoViewModel> listuserAdmin = listeUser.Select(x => new UtilisateurPersoViewModel()
            //{
            //    AdresseCourriel = x.AdresseCourriel,
            //    Nom = x.Nom,
            //    Prenom = x.Prenom,
            //    RegistrerDate = x.RegistrerDate,
            //    Role =providerlogin.Roles.ToList().Find
            //});
            return View(listeUser);
        }

        [HttpGet]
        public IActionResult ModifierUtilisateur(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult ModifierUtilisateur()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AjouterUtilisateur()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AjouterUtilisateur2()
        {
            return View();
        }


        [HttpGet]
        public IActionResult SupprimerUtilisateur()
        {

            return View();
        }


        [HttpPost]
        public IActionResult SupprimerUtilisateur(int id)
        {
            return View();
        }
    }
}