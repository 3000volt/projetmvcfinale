using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class CorrigeController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;

        //Controlleur
        public CorrigeController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
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
            ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");

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
                SqlDataReader reader;

                corrige.DateInsertion = DateTime.Today;
                provider.Add(corrige);
                await provider.SaveChangesAsync();
                HttpContext.Session.SetString("Corrige", JsonConvert.SerializeObject(corrige));//pour aller le chercher pour l'upload
                
                //Ajouter l'id du corrigé a l'exercice correspondant
                string query = @"UPDATE Exercice SET idCorrige ='" + corrige.Idcorrige + "' WHERE Idexercice = '" + corrige.Idexercice + "'";
                SqlCommand commande = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                reader = commande.ExecuteReader();
                sqlConnection.Close();
            }
            return RedirectToAction(nameof(ListeCorrige));
        }
        [HttpGet]
        public IActionResult UploadCorrige()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lien"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadCorrige(IFormFile Lien)
        {
            SqlDataReader reader;

            Corrige corrige = JsonConvert.DeserializeObject<Corrige>(this.HttpContext.Session.GetString("corrige"));

            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", Lien.FileName);

            string query = @"UPDATE Corrige SET Lien ='" + chemin + "' WHERE NomCorrige = '" + corrige.CorrigeDocNom + "'";
            SqlCommand commande = new SqlCommand(query, sqlConnection);

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }

            sqlConnection.Open();
            reader = commande.ExecuteReader();
            return Ok("Fichier téléversé avec succès!");
        }
    }
}