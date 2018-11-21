using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace projetmvcfinale.Controllers
{
    public class NoteDeCoursController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}