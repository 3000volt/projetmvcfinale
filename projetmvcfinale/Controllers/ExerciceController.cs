using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class ExerciceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AjoutExercice()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AjoutExercice(Exercice exercice)
        {
            return View();
        }

        public async Task<IActionResult> SupprimerExercice(string id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SupprimerExercicePost(string id)
        {
            return View();
        }


    }
}