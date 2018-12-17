using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            try
            {
                //Les listes necessaires
                ViewBag.ListCategorie = this.provider.Categorie.ToList();
                ViewBag.Categorie = new Categorie();
                List<SousCategorieViewModel> listeSousCat = new List<SousCategorieViewModel>();
                foreach (SousCategorie s in this.provider.SousCategorie.ToList())
                {
                    listeSousCat.Add(new SousCategorieViewModel()
                    {
                        IdSousCategorie = s.IdSousCategorie,
                        NomSousCategorie = s.NomSousCategorie,
                        NomCategorie = this.provider.Categorie.ToList().Find(x => x.IdCateg == s.IdCateg).NomCategorie
                    });
                }
                ViewBag.ListSousCategorie = listeSousCat;
                ViewBag.ListeNiveau = this.provider.Niveau.ToList();
                ViewBag.Difficulte = this.provider.SousCategorie.ToList();
                ViewBag.SelectCategorie = this.provider.Categorie.ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }
            
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
        public string ModifierCategorie(int idCategorie)//
        {
            string categorie = this.provider.Categorie.ToList().Find(x => x.IdCateg == idCategorie).NomCategorie;
            return categorie;
        }

        [HttpPost]
        public IActionResult ModifierCategorie([FromBody][Bind("idCateg,NomCategorie")]Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                Categorie categorieUpdate = this.provider.Categorie.ToList().Find(x => x.IdCateg == categorie.IdCateg);
                categorieUpdate.NomCategorie = categorie.NomCategorie;
                this.provider.Update(categorieUpdate);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }


        //Sous catégorie IdSousCategorie
        [HttpPost]
        public IActionResult CreerSousCategorie([FromBody][Bind("NomCategorie")] SousCategorie sousCategorie)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Ajouter a la bd
                this.provider.Add(sousCategorie);
                this.provider.SaveChanges();
                return Ok("élément ajouté avec succès");
            }
            return BadRequest("Erreur de modification");
        }

        [HttpPost]
        public int VoirSousDocumentsCategorie(int idSousCateg)
        {
            //Voir les notes de cours
            int nbrNote = this.provider.NoteDeCours.ToList().Where(x => x.IdSousCategorie == idSousCateg).Count();
            //Retourner le total
            return nbrNote;
        }

        [HttpPost]
        public void SupprimerSousCategorie(int idSousCategorie)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Enlever de la bd
                SousCategorie SousCateg = this.provider.SousCategorie.ToList().Find(x => x.IdSousCategorie == idSousCategorie);
                this.provider.Remove(SousCateg);
                this.provider.SaveChanges();
            }
        }

        [HttpGet]
        public string ModifierSousCategorie(int idSousCategorie)
        {
            string sousCategorie = this.provider.SousCategorie.ToList().Find(x => x.IdSousCategorie == idSousCategorie).NomSousCategorie;
            return sousCategorie;
        }

        [HttpPost]
        public IActionResult ModifierSousCategorie([FromBody][Bind("IdSousCategorie,NomSousCategorie")]SousCategorie sousCategorie)
        {
            if (ModelState.IsValid)
            {
                SousCategorie sousCategorieUpdate = this.provider.SousCategorie.ToList().Find(x => x.IdSousCategorie == sousCategorie.IdSousCategorie);
                sousCategorieUpdate.NomSousCategorie = sousCategorie.NomSousCategorie;
                this.provider.Update(sousCategorieUpdate);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }

        //Partie niveau
        [HttpPost]
        public int VoirDocumentsNiveau(int idNiveau)
        {
            //Voir les exercices
            int nbrExercice = this.provider.Exercice.ToList().Where(x => x.Idexercice == idNiveau).Count();
            //Retourner le total
            return nbrExercice;
        }

        [HttpPost]
        public void SupprimerNiveau(int idNiveau)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Enlever de la bd
                Niveau niv = this.provider.Niveau.ToList().Find(x => x.IdDifficulte == idNiveau);
                this.provider.Remove(niv);
                this.provider.SaveChanges();
            }
        }

        [HttpGet]
        public string ModifierNiveau(int idNiveau)//
        {
            string niveau = this.provider.Niveau.ToList().Find(x => x.IdDifficulte == idNiveau).NiveauDifficulte;
            return niveau;
        }

        [HttpPost]
        public IActionResult ModifierNiveau([FromBody][Bind("IdDifficulte,NiveauDifficulte")]Niveau niveau)
        {
            if (ModelState.IsValid)
            {
                Niveau niveauUpddate = this.provider.Niveau.ToList().Find(x => x.IdDifficulte == niveau.IdDifficulte);
                niveauUpddate.NiveauDifficulte = niveau.NiveauDifficulte;
                this.provider.Update(niveauUpddate);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }

        [HttpPost]
        public IActionResult CreerNiveau([FromBody][Bind("NomNiveau")] Niveau niveau)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Ajouter a la bd
                this.provider.Add(niveau);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }
    }
}