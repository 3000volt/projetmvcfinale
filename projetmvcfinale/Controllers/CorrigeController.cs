using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class CorrigeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AjouterCorrige()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AjouterCorrige([Bind("Idcorrige,CorrigeDocNom,Lien,DateInsertion,Idexercice")] Corrige corrige)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}