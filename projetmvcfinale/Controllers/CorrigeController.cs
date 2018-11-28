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
    public class CorrigeController : Controller
    {

        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;

        public CorrigeController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult ListeCorrige()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AjouterCorrige()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="corrige"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AjouterCorrige([Bind("Idcorrige,CorrigeDocNom,Lien,DateInsertion,Idexercice")] Corrige corrige)
        {
            if (ModelState.IsValid)
            {
                corrige.DateInsertion = DateTime.Today;

                provider.Add(corrige);
                await provider.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListeCorrige));
        }
        [HttpGet]
        public IActionResult UploadCorrige()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCorrige(IFormFile Lien)
        {
            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents/Corrige", Lien.FileName);

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }
            return Ok("Fichier téléversé avec succès!");
        }
    }
}