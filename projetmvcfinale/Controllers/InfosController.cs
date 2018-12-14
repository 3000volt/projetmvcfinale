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

        [HttpPost]
        public int VoirDocumentsCategorie(int idCateg)
        {
            //Voir les notes de cours
            int nbrCours = this.provider.NoteDeCours.ToList().Where(x => x.IdCateg == idCateg).Count();
            //Voir les exercices
            int nbrExercice = this.provider.Exercice.ToList().Where(x => x.IdCateg == idCateg).Count();
            //calcluler le total
            int total = nbrCours + nbrExercice;
            //Retourner le total
            return total;
        }

        [HttpPost]
        public void SupprimerCategorie(int idCategorie)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Enlever de la bd
                Categorie categ = this.provider.Categorie.ToList().Find(x => x.IdCateg == idCategorie);
                this.provider.Remove(categ);
                this.provider.SaveChanges();
            }
        }

        [HttpGet]
        public IActionResult ModifierCategorie(int idCategorie)
        {
            Categorie categorie = this.provider.Categorie.ToList().Find(x=>x.IdCateg == idCategorie);
            return Json(categorie);
        }

        [HttpPost]
        public IActionResult ModifierCategorie([FromBody][Bind("idCateg,NomCategorie")]Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                Categorie categorieUpdate = this.provider.Categorie.ToList().Find(x=>x.IdCateg == categorie.IdCateg);
                categorieUpdate.NomCategorie = categorie.NomCategorie;
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }

            return BadRequest("Erreur de modification");

        }

    }
}