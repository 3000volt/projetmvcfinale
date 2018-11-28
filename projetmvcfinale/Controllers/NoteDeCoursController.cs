using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class NoteDeCoursController : Controller
    {
        private readonly ProjetFrancaisContext _context;
        
        public NoteDeCoursController(ProjetFrancaisContext context)
        {
            _context = context;
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
                _context.Add(note);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}