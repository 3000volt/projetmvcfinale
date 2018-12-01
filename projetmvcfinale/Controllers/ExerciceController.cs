using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projetmvcfinale.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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
            this.provider.Niveau.ToList();

            int i = 0;
            this.provider.Categorie.ToList();
            int y = 0;
            //Options de type d,exercice
            List<string> listeTypes = new List<string>();
            listeTypes.Add("Normal");
            listeTypes.Add("Interactif");
            ViewBag.Niveau = new SelectList(this.provider.Niveau, "IdDifficulte", "NiveauDifficulte");
            ViewBag.Categorie = new SelectList(this.provider.Categorie, "IdCateg", "NomCategorie");
            ViewBag.Type = listeTypes.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.ToString()
                                  });
            //https://stackoverflow.com/questions/20380994/convert-list-to-ienumerableselectlistitem
            return View();
            //source:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1, https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core
        }
        /// <summary>
        /// Ajouter un exercice
        /// </summary>
        /// <param name="exercice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AjoutExercice(ExerciceVM exerciceVM)
        {
            if (ModelState.IsValid)
            {
                //Convertion en Exercice
                Exercice exercice = new Exercice()
                {
                    NomExercices = exerciceVM.NomExercices,
                    DateInsertion = DateTime.Today,
                    TypeExercice = exerciceVM.TypeExercice,
                    AdresseCourriel = this.HttpContext.User.Identity.Name,
                    IdDifficulte = exerciceVM.IdDifficulte,
                    IdCateg = exerciceVM.IdCateg
                };
                string test = exercice.AdresseCourriel;
                //Ajouter au contexte
                provider.Add(exercice);
                await provider.SaveChangesAsync();
                //Envoyer vers l'autre page
                if (exercice.TypeExercice == "Interactif")
                {
                    ViewBag.Type = "Interactif";
                }
                else if (exercice.TypeExercice == "Normal")
                {
                    ViewBag.Type = "Normal";
                }
                //Mettre l'exercice dans une session
                InsertionExercice insertion = new InsertionExercice()
                {
                    exercice = exercice,
                    //listeLignes = new SortedList<int, LignePerso>()
                    listeLignes = new List<LignePerso>()
                };
                this.HttpContext.Session.SetString("Exercice", JsonConvert.SerializeObject(insertion));
                //Envoyer vers la vue pour continuer la creation selon le type
                return View("CompleterCreation", exerciceVM);
            }

            return BadRequest("Erreur dans l'insertion de l'exercice");
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
            return File("(~wwwroot/DocumentsExercices" + fileName, "application/vnd.ms-word");

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

        [HttpPost]
        public void CreationLigne([FromBody][Bind("NumeroQuestion,Ligne")]LignePerso ligne)
        {
            //Instancier la liste des choix
            ligne.listeChoixReponses = new List<ChoixDeReponse>();
            //Associer la ligne en cours a la session
            this.HttpContext.Session.SetString("Ligne", JsonConvert.SerializeObject(ligne));
            LignePerso ligne2 = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
            int i = 0;

        }

        [HttpPost]
        public void AjoutChoixReponse([FromBody][Bind("ChoixDeReponse1,Response,NoOrdre")]ChoixDeReponseTest choix)
        {
            //LignePerso test = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
            InsertionExercice insertion = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            //Transformer le choix pour qu'il puisse entrer dans l aBD
            ChoixDeReponse choixDeReponse = new ChoixDeReponse()
            {
                ChoixDeReponse1 = choix.ChoixDeReponse1,
                Response = choix.Response,
                NoOrdre = choix.NoOrdre,
                //IdLigne = this.provider.LigneTestInteractif.ToList().Find(x => x.Idexercice == insertion.exercice.Idexercice && x.NumeroQuestion == ligne.NumeroQuestion).IdLigne
            };
            // if (this.HttpContext.Session.GetString("Ligne") != null)
            // {
            LignePerso ligne = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
            //}
            ligne.listeChoixReponses.Add(choixDeReponse);
            //Mettre a jour les sessions
            this.HttpContext.Session.SetString("Ligne", JsonConvert.SerializeObject(ligne));

            //List<LignePerso> liste = new List<LignePerso>();
            //if (this.HttpContext.Session.GetString("ListeLigne") != null)
            //{
            //    //Associer la liste a celle ci
            //    liste = JsonConvert.DeserializeObject<List<LignePerso>>(this.HttpContext.Session.GetString("ListeLigne"));
            //}
            //liste.Add(ligne);
            //this.HttpContext.Session.SetString("ListeLigne", JsonConvert.SerializeObject(liste));
            //int i = 0;
        }



        [HttpPost]
        public void TerminerPhrase()
        {
            //Prendre la phrase dans la session
            LignePerso ligne = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
            //Ajouter cette ligne a la liste de la session de l'exercice en cours
            InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            //exercice.listeLignes.Add(ligne.NumeroQuestion, ligne);
            exercice.listeLignes.Add(ligne);
            //Mettre a jour la session
            this.HttpContext.Session.SetString("Exercice", JsonConvert.SerializeObject(exercice));
        }

        [HttpPost]
        public void EnvoyerExercice()
        {
            //Envoyer le contenue de l'insertion vers la BD
            InsertionExercice Insertionexercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            //Tout mettre dans la BD
            //Pour chaques lignes
            foreach (LignePerso ligne in Insertionexercice.listeLignes)
            {
                //Ajuster la ligne au format de la BD
                LigneTestInteractif ligneInteractif = new LigneTestInteractif()
                {
                    NumeroQuestion = ligne.NumeroQuestion,
                    Ligne = ligne.Ligne,
                    Idexercice = Insertionexercice.exercice.Idexercice
                };
                //Ajouter la ligne a la bd
                this.provider.Add(ligneInteractif);
                //pour chaque choix de cette ligne
                foreach (ChoixDeReponse choix in ligne.listeChoixReponses)
                {
                    choix.IdLigne = ligneInteractif.IdLigne;
                    //Ajouter a la BD
                    this.provider.Add(choix);
                }
            }
            //Sauvegarder les données insérées
            this.provider.SaveChanges();
        }

        [HttpPost]
        public bool VerifierNumero(int numero)
        {
            bool disponible = true;
            //Voir si la session existe
            if (this.HttpContext.Session.GetString("Exercice") != null)
            {
                //Associer la liste
                InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
                //si le numero existe deja dans la sortedList
                //if (exercice.listeLignes.ContainsKey(numero))
                if (exercice.listeLignes.Any(x => x.NumeroQuestion == numero))
                {
                    disponible = false;
                }
            }
            //Si la session n'existe pas, c'ests ur que le numeroe st disponible


            return disponible;
        }
    }
}