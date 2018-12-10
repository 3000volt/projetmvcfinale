using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using projetmvcfinale.Models;
using projetmvcfinale.Models.Authentification;

namespace projetmvcfinale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtilisateurController : Controller
    {


        //propriétés du controlleur
        private readonly LoginDbContext providerlogin;
        private readonly UserManager<LoginUser> _userManager;
        private readonly IConfiguration Configuration;
        private readonly ProjetFrancaisContext provider;
        private readonly RoleManager<LoginRole> _roleManager;


        //constructeur
        public UtilisateurController(IConfiguration configuration, LoginDbContext log, RoleManager<LoginRole> roleManager, UserManager<LoginUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.Configuration = configuration;
            this.providerlogin =log;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }

        /// <summary>
        /// Méthode qui afiche la liste des users
        /// </summary>
        /// <returns></returns>
        public IActionResult ListeUtilisateur()
        {
            //liste des utilisateurs
            List<Utilisateur> listeUser = this.provider.Utilisateur.ToList();

            //liste des utlisateurs personnaliser
            List<UtilisateurPersoViewModel> listuserAdmin = listeUser.Select(x => new UtilisateurPersoViewModel()
            {
                AdresseCourriel = x.AdresseCourriel,
                Nom = x.Nom,
                Prenom = x.Prenom,
                RegistrerDate = x.RegistrerDate    ,
                Role = providerlogin.Roles.ToList().Find(y => y.Id == providerlogin.UserRoles.ToList().Find(t => t.UserId == providerlogin.Users.ToList().Find(z => z.Email == x.AdresseCourriel).Id).RoleId).Name
            }).ToList();
            return View(listuserAdmin);
        }

        /// <summary>
        /// Méthode get du modifier utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]

        public IActionResult ModifierUtilisateur(string email)
        {
            Utilisateur user = this.provider.Utilisateur.ToList().Find(m => m.AdresseCourriel == email);
            UtilisateurPersoViewModel useredit = new UtilisateurPersoViewModel() { AdresseCourriel = user.AdresseCourriel, Nom = user.Nom, Prenom = user.Prenom, RegistrerDate = user.RegistrerDate, Role = providerlogin.Roles.ToList().Find(y => y.Id == providerlogin.UserRoles.ToList().Find(t => t.UserId == providerlogin.Users.ToList().Find(z => z.Email == user.AdresseCourriel).Id).RoleId).Name };
            useredit.Roles = this._roleManager.Roles.Select<LoginRole, SelectListItem>(
              r =>
              new SelectListItem()
              {
                  Text = r.Name,
                  Value = r.Name
              }
          ).ToList();

            return View(useredit);
        }


        /// <summary>
        /// Méthode qui permet d'enregistrer les modifications apporter 
        /// sur un utilisateur suite à une modification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierUtilisateurpost(UtilisateurPersoViewModel util)
        {

            Utilisateur user = new Utilisateur() {AdresseCourriel=util.AdresseCourriel,Nom=util.Nom,Prenom=util.Prenom,RegistrerDate=util.RegistrerDate };
            if (ModelState.IsValid)
            {
                try
                {
                    this.provider.Update(user);
                    await this.provider.SaveChangesAsync();

                    // THIS LINE IS IMPORTANT
                    var userId =providerlogin.Users.ToList().Find(x=>x.Email==user.AdresseCourriel).Id;
                    var oldRoleId = providerlogin.UserRoles.ToList().Find(x=>x.UserId== userId).RoleId;
                    var oldRoleName = providerlogin.Roles.ToList().Find(x=>x.Id==oldRoleId).Name;

                    if (oldRoleName != util.Role)
                    {
                        //dissocie l'ancien role d'un utilisateur si l'admin a changé son role
                        await _userManager.RemoveFromRoleAsync(user: providerlogin.Users.ToList().Find(x => x.Email == user.AdresseCourriel),role:oldRoleName);

                        //Associe le nouveau role assigné par l'admin à un utilisateur
                        await _userManager.AddToRoleAsync(providerlogin.Users.ToList().Find(x => x.Email == user.AdresseCourriel), util.Role);                      
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    //page d'erreur
                    return null;
                }
                return RedirectToAction(nameof(ListeUtilisateur));
            }
            return View(util);

        }


        /// <summary>
        /// Méthode get du supprimer utilisateur
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        public IActionResult SupprimerUtilisateur(string email)
        {
            Utilisateur user = this.provider.Utilisateur.ToList().Find(m=>m.AdresseCourriel==email);
            UtilisateurPersoViewModel userdelete = new UtilisateurPersoViewModel() {AdresseCourriel=user.AdresseCourriel,Nom=user.Nom,Prenom=user.Prenom,RegistrerDate=user.RegistrerDate,Role= providerlogin.Roles.ToList().Find(y => y.Id == providerlogin.UserRoles.ToList().Find(t => t.UserId == providerlogin.Users.ToList().Find(z => z.Email == user.AdresseCourriel).Id).RoleId).Name };
            return View(userdelete);
        }

        /// <summary>
        /// Méthode post du supprimer utilisateur
        /// permet d'enregistrer les modifications suit à une suppresion
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerUtilisateurpost(string email)
        {
            this.provider.Utilisateur.Remove(this.provider.Utilisateur.ToList().Find(x=>x.AdresseCourriel==email));
            await this.provider.SaveChangesAsync();
            this.providerlogin.Users.Remove(this.providerlogin.Users.ToList().Find(x => x.Email == email));
            await this.providerlogin.SaveChangesAsync();
            return  RedirectToAction(nameof(ListeUtilisateur));
        }

    }
}