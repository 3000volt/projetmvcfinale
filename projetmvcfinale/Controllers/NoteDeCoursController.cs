using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AjouterNote()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AjouterCours([Bind("IdDocument,NomNote,Lien,DateInsertion,AdresseCourriel,IdCateg,IdSousCategorie")] NoteDeCours note)
        {
            if(ModelState.IsValid)
            {
                provider.Add(note);
                await provider.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}