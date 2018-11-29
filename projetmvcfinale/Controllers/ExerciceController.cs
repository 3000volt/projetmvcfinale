using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace projetmvcfinale.Controllers
{
    public class ExerciceController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;

        public ExerciceController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult ListeExercice()
        {
            List<Exercice> listeExercice = this.provider.Exercice.ToList();
            return View(listeExercice);
        }
        /// <summary>
        /// Affiche la vue pour ajouter un exercice
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AjoutExercice()
        {
            return View();

            //source:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1, https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core

        }
        /// <summary>
        /// Ajouter un exercice
        /// </summary>
        /// <param name="exercice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AjoutExercice([Bind("Idexercice,NomExercices,Lien,TypeExercice,AdresseCourriel,IdDifficulte,Idcorrige,IdDocument,IdCateg")]Exercice exercice)
        {
            if(ModelState.IsValid)
            {
                exercice.DateInsertion = DateTime.Today;

                provider.Add(exercice);
                await provider.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListeExercice));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UploadExercice()
        {
            return View();
        }
        /// <summary>
        /// téléverser le fichier dans le dossier d'exercice
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadExercice(IFormFile Lien)
        { 
            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");
                
                    var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents/Exercices", Lien.FileName);

                    using (var stream = new FileStream(chemin, FileMode.Create))
                    {
                        await Lien.CopyToAsync(stream);

                    }
                    return Ok("Fichier téléversé avec succès!");    
        }

        public async Task<IActionResult> DownloadExercice(string fileName)
        {
            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents/Exercices", fileName);

            var memoire = new MemoryStream();

            using (var stream = new FileStream(chemin, FileMode.Open))
            {
                await stream.CopyToAsync(memoire);
            }
            memoire.Position = 0;
            return File("(~wwwroot/DocumentsExercices" + fileName,"application/vnd.ms-word");

        }
        /// <summary>
        /// Affiche la vue pour supprimer un exercice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SupprimerExercice(string id)
        {
            return View();
        }
        /// <summary>
        /// Supprimer un exercice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SupprimerExercicePost(string id)
        {
            return View();
        }
    }
}