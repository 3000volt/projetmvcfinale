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
    public class NoteDeCoursController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;

        public NoteDeCoursController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Pour permettre au ViewBag contenant les categories d'etre accessible en tout temps    
            base.OnActionExecuted(context);
            ViewBag.Categories = this.provider.Categorie.ToList();
            ViewBag.souscatégorie = this.provider.SousCategorie.ToList();
            //Merci https://stackoverflow.com/questions/40330391/set-viewbag-property-in-the-constructor-of-a-asp-net-mvc-core-controller
        }

        public IActionResult ListeNoteDeCours(string search)
        {
        //    List<NoteDeCours> listeNote = this.provider.NoteDeCours.ToList();
        //    return View(listeNote);
            return View(this.provider.NoteDeCours.Where(x => x.NomNote.StartsWith(search) || search == null).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjouterNote()
        {
            ViewBag.IdCateg = new SelectList(this.provider.Categorie.ToList(), "IdCateg", "NomCategorie");

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjouterNote([Bind("NomNote,IdCateg,SousCategorie,Lien")] NotesViewModel noteVM)
        {
            if(ModelState.IsValid)
            {
                //Transferer en note
                NoteDeCours note = new NoteDeCours()
                {
                    NomNote = noteVM.NomNote,
                    IdCateg = noteVM.IdCateg,
                    IdSousCategorie = this.provider.SousCategorie.ToList().Find(x=>x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie,
                    Lien = noteVM.Lien.FileName,
                    AdresseCourriel = this.HttpContext.User.Identity.Name,
                    DateInsertion = DateTime.Now

                };
                //note.DateInsertion = DateTime.Today;
                //note.AdresseCourriel = JsonConvert.DeserializeObject<Utilisateur>(this.HttpContext.Session.GetString("user")).AdresseCourriel;
                provider.Add(note);
                await provider.SaveChangesAsync();
                //HttpContext.Session.SetString("NoteDeCours", JsonConvert.SerializeObject(note));//pour aller le chercher pour l'upload
                //Insérer dans la BD le document
                if (note.Lien == null || note.Lien.Length == 0)
                    return Content("Aucun fichier sélectionné");

                var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.Lien);

                //ajouter le lien à la base de données
                note.Lien = chemin;
                //provider.Exercice.Update(ex);
                await provider.SaveChangesAsync();

                using (var stream = new FileStream(chemin, FileMode.Create))
                {
                    await noteVM.Lien.CopyToAsync(stream);
                }

                return RedirectToAction(nameof(ListeNoteDeCours));
            }

            return RedirectToAction(nameof(ListeNoteDeCours));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UploadNote()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lien"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadNote(IFormFile Lien)
        {
            NoteDeCours note = JsonConvert.DeserializeObject<NoteDeCours>(this.HttpContext.Session.GetString("NoteDeCours"));

            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", Lien.FileName);

            note.Lien = chemin;
            provider.NoteDeCours.Update(note);
            await provider.SaveChangesAsync();

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }

            return Ok("Fichier téléversé avec succès!");
        }

        [HttpPost]
        public List<string> RetirerSousCateg(int id)
        {
            List<string> sousCategorie = new List<string>();
            //Trouver tout les
            sousCategorie = this.provider.SousCategorie.ToList().FindAll(x => x.IdCateg == id).Select(x => x.NomSousCategorie).ToList();
            //Retourner la liste
            return sousCategorie;
        }
    }
}