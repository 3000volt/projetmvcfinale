using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class InfosController : Controller//
    {
        //Propriétés du controlleur
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;

        //Constructeur
        public InfosController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
            //ViewBag.Categories = this.provider.Categorie.ToList();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Pour permettre au ViewBag contenantn les categores d'etre accessible en tout temps    
            base.OnActionExecuted(context);
            ViewBag.Categories = this.provider.Categorie.ToList();
            ViewBag.Notes = this.provider.NoteDeCours.ToList();
            ViewBag.NiveauDif = this.provider.Niveau.ToList();
            ViewBag.souscatégorie = this.provider.SousCategorie.ToList();
            //Merci https://stackoverflow.com/questions/40330391/set-viewbag-property-in-the-constructor-of-a-asp-net-mvc-core-controller
        }

        public IActionResult GererInfos()
        {
            //Les listes necessaires
            ViewBag.ListCategorie = this.provider.Categorie.ToList();
            ViewBag.Categorie = new Categorie();
            return View();
        }

        [HttpPost]
       public IActionResult CreerCategorie([FromBody][Bind("NomCategorie")] Categorie categorie)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Ajouter a la bd
                this.provider.Add(categorie);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }
    }
}