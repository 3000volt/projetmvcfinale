using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class NoteDeCoursController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;

        public NoteDeCoursController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult ListeNoteDeCours()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AjouterNote()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AjouterNote([Bind("IdDocument,NomNote,Lien,AdresseCourriel,IdCateg,IdSousCategorie")] NoteDeCours note)
        {
            if(ModelState.IsValid)
            {
                note.DateInsertion = DateTime.Today;

                provider.Add(note);
                await provider.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListeNoteDeCours));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        [HttpPost]
        public async Task<IActionResult> UploadNote(IFormFile Lien)
        {
            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents/NoteDeCours", Lien.FileName);

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }
            return Ok("Fichier téléversé avec succès!");
        }
    }
}