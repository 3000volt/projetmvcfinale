using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class CorrigeController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;
        private List<Corrige> listeCor;

        //Controlleur
        public CorrigeController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
            this.listeCor = this.provider.Corrige.ToList();
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
        public IActionResult ListeCorrige(string search)
        {
            return View(this.provider.Corrige.ToList());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjouterCorrige()
        {
            ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="corrige"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjouterCorrige([Bind("CorrigeDocNom,Lien,Idexercice")] CorrigeViewModel corrigeVM)
        {
            if (ModelState.IsValid)
            {
                //SqlDataReader reader;

                Corrige corrige = new Corrige()
                {
                    CorrigeDocNom = corrigeVM.CorrigeDocNom,
                    Lien = corrigeVM.Lien.FileName,
                    DateInsertion = DateTime.Now,
                    Idexercice = corrigeVM.Idexercice

                };

                provider.Add(corrige);
                await provider.SaveChangesAsync();
                // listeCor.Add(corrige);
                /*HttpContext.Session.SetString("Corrige", JsonConvert.SerializeObject(corrige));*///pour aller le chercher pour l'upload

                Exercice ex = this.provider.Exercice.ToList().Find(x => x.Idexercice == corrige.Idexercice);
                ex.Idcorrige = corrige.Idcorrige;
                provider.Exercice.Update(ex);
                await provider.SaveChangesAsync();


                //Insérer dans la BD le document
                if (corrige == null || corrige.Lien.Length == 0)
                    return Content("Aucun fichier sélectionné");

                var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", corrige.Lien);

                corrige.Lien = chemin;
                provider.Corrige.Update(corrige);
                await provider.SaveChangesAsync();

                using (var stream = new FileStream(chemin, FileMode.Create))
                {
                    await corrigeVM.Lien.CopyToAsync(stream);
                }

                return RedirectToAction(nameof(ListeCorrige));
            }
            return RedirectToAction(nameof(ListeCorrige));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UploadCorrige()
        {
            ViewBag.IdCorrige = new SelectList(this.provider.Corrige, "IdCorrige", "NomCorrige");
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lien"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadCorrige(IFormFile Lien)
        {
            Corrige corrige = JsonConvert.DeserializeObject<Corrige>(this.HttpContext.Session.GetString("corrige"));

            //var idCor = this.provider.Corrige.FindAsync(corrige.Idcorrige);

            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", Lien.FileName);

            corrige.Lien = chemin;
            provider.Corrige.Update(corrige);
            await provider.SaveChangesAsync();

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }

            return Ok("Fichier téléversé avec succès!");
        }

        [HttpGet]
        public ActionResult ModifierCorrige(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ModifierCorrigePost(string id)
        {
            return View();
        }
        /// <summary>
        /// Afficher la vue avant de supprimer un corrige
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> SupprimerCorrige(string id)
        {
            if(ModelState.IsValid)
            {
              if (id == null)
                return NotFound();

               //aller chercher le cours dans le contexte
                Corrige cr = await provider.Corrige.FindAsync(id);

                //vérifier si le cours est null
                if (cr == null)
                return NotFound();
  
              return View(cr);
            }
            return BadRequest("Erreur");
            
        }

        /// <summary>
        /// Supprimer un corrige de la liste
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> SupprimerCorrigePost(string id)
        {
            if(ModelState.IsValid)
            {
              Corrige corrige = await provider.Corrige.FindAsync(id);
              string chemin = corrige.Lien;
              //trouver l'exercice correspondant & mettre l'id du corrige a null
              Exercice ex = await provider.Exercice.FindAsync(corrige.Idexercice);
              ex.Idcorrige = null;
              provider.Update(ex);
                
                //supprimer le corrige de la BD
                provider.Corrige.Remove(corrige);
                
                //supprimer le fichier
                if (System.IO.File.Exists(chemin))
                {
                    System.IO.File.Delete(chemin);
                }
                await provider.SaveChangesAsync();
              return RedirectToAction(nameof(ListeCorrige));
            }
            return BadRequest("Erreur");
        }
        //source:https://stackoverflow.com/questions/22650740/asp-net-mvc-5-delete-file-from-server
    }
}