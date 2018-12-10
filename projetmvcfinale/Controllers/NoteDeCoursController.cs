using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult ListeNoteDeCours()
        {
            List<NoteDeCours> listeNote = this.provider.NoteDeCours.ToList();
            return View(listeNote);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjouterNote()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjouterNote([Bind("IdDocument,NomNote,IdCateg,IdSousCategorie")] NoteDeCours note)
        {
            if(ModelState.IsValid)
            {
                note.DateInsertion = DateTime.Today;
                note.AdresseCourriel = JsonConvert.DeserializeObject<Utilisateur>(this.HttpContext.Session.GetString("user")).AdresseCourriel;
                provider.Add(note);
                await provider.SaveChangesAsync();
                HttpContext.Session.SetString("NoteDeCours", JsonConvert.SerializeObject(note));//pour aller le chercher pour l'upload
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
    }
}