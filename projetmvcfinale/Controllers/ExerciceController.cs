using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace projetmvcfinale.Controllers
{
    public class ExerciceController : Controller
    {
        private readonly ProjetFrancaisContext _context;

        public IActionResult ListeExercice()
        {
            return View();
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
        public async Task<IActionResult> AjoutExercice([Bind("Idexercice,NomExercices,Lien,DateInsertion,TypeExercice,AdresseCourriel,IdDifficulte,Idcorrige,IdDocument,IdCateg")]Exercice exercice,IFormFile file)
        {
            if(ModelState.IsValid)
            {
                _context.Add(exercice);
                await _context.SaveChangesAsync();
      
                if(file != null && file.Length > 0)
                {
                    using (var stream = new FileStream(Path.GetTempPath(), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest("Il y a une erreur avec le fichier");
                }
            }

            return RedirectToAction(nameof(ListeExercice));
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