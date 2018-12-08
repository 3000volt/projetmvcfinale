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
using System.Data.SqlClient;

namespace projetmvcfinale.Controllers
{
    public class ExerciceController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;

        //Constructeur
        public ExerciceController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
        }
        /// <summary>
        /// Afficher la liste d'exercices
        /// </summary>
        /// <returns></returns>
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
        public IActionResult AjoutExercice()
        {
            this.provider.Niveau.ToList();

            this.provider.Categorie.ToList();

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
                    AdresseCourriel = JsonConvert.DeserializeObject<Utilisateur>(this.HttpContext.Session.GetString("user")).AdresseCourriel,
                    IdDifficulte = exerciceVM.IdDifficulte,
                    IdCateg = exerciceVM.IdCateg
                };

                HttpContext.Session.SetString("exercice", JsonConvert.SerializeObject(exercice));//pour aller le chercher pour l'upload

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
                //Pour si une phras ese continue
                this.HttpContext.Session.SetString("PhraseASuivre", "Innactif");
                //Envoyer vers la vue pour continuer la creation selon le type
                return View("CompleterCreation", exerciceVM);
            }
            return BadRequest("Erreur dans l'insertion de l'exercice");
        }
        /// <summary>
        /// Afficher la vue pour téléverser un fichier d'exercice
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
            SqlDataReader reader;

            Exercice ex = JsonConvert.DeserializeObject<Exercice>(this.HttpContext.Session.GetString("exercice"));

            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", Lien.FileName);

            //ajouter le lien à la base de données
            string query = @"UPDATE Exercice SET Lien ='" + chemin + "' WHERE NomExercices = '" + ex.NomExercices + "'";
            SqlCommand commande = new SqlCommand(query, sqlConnection);

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }

            sqlConnection.Open();
            reader = commande.ExecuteReader();
            sqlConnection.Close();
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
            if (id == null)
                return NotFound();

            //aller chercher le cours dans le contexte
            Exercice ex = await provider.Exercice.FindAsync(id);

            //vérifier si le cours est null
            if (ex == null)
                return NotFound();

            return View(ex);
        }
        /// <summary>
        /// Supprimer un exercice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SupprimerExercicePost(string id)
        {
            Exercice exercice = await provider.Exercice.FindAsync(id);
            provider.Exercice.Remove(exercice);
            await provider.SaveChangesAsync();
            return RedirectToAction(nameof(ListeExercice));
        }

        [HttpPost]
        public void CreationLigne([FromBody][Bind("NumeroQuestion,Ligne,ChoixDeReponse1,Response,NoOrdre")]LignePerso ligne)
        {
            //Transformer le choix pour qu'il puisse entrer dans l aBD
            List<ChoixDeReponse> choixDeReponse = new List<ChoixDeReponse>();
            if (ligne.listeChoixReponses2 != null)
            {
                foreach (ChoixDeReponseTest c in ligne.listeChoixReponses2)
                {
                    choixDeReponse.Add(new ChoixDeReponse()
                    {
                        ChoixDeReponse1 = c.ChoixDeReponse1,
                        Response = c.Response,
                        NoOrdre = c.NoOrdre,
                    });
                }
            }
            //Si la session est vide (Première partie de phrase envoyé)
            LignePerso ligneSession = new LignePerso();
            if (this.HttpContext.Session.GetString("Ligne") == null)
            {
                ligneSession.NumeroQuestion = ligne.NumeroQuestion;
                ligneSession.Ligne = ligne.Ligne + "(?)";
                ligneSession.listeChoixReponses = choixDeReponse;
            }
            //S'il s'agit d'une suite de phrase
            else
            {
                ligneSession = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
                //Vérifier s'il s'agit d'une mise a jour des choix de réponses ou une premeire fois
                if (ligneSession.listeChoixReponses.Last().NoOrdre == choixDeReponse.Last().NoOrdre)
                {
                    //Retirer les derniers choix de réponse
                    ligneSession.listeChoixReponses.RemoveAll(x => x.NoOrdre == choixDeReponse.Last().NoOrdre);
                }
                else
                {
                    //Associer la ligne au complete s'il s'agit de la 1ere fois
                    ligneSession.Ligne = ligneSession.Ligne + ligne.Ligne + "(?)";
                }
                //Insérer les choix de réponses   
                foreach (ChoixDeReponse c in choixDeReponse)
                {
                    ligneSession.listeChoixReponses.Add(c);
                }
            }
            //Instancier la liste des choix
            //Associer la ligne en cours a la session
            this.HttpContext.Session.SetString("Ligne", JsonConvert.SerializeObject(ligneSession));
        }

        [HttpPost]
        public void RetirerPhrase()
        {
            //Si une session est existant, lui mettre fin
            if (this.HttpContext.Session.GetString("Ligne") != null)
            {
                this.HttpContext.Session.Remove("Ligne");
            }

        }

        //TerminerLigne
        [HttpPost]
        public void TerminerLigne([FromBody][Bind("NumeroQuestion,Ligne")]LignePerso ligne)
        {

            LignePerso lignePerso = new LignePerso();
            //Si aucune partie de phrase n'a été envoyé (phrase sans choix de réponse)
            if (this.HttpContext.Session.GetString("Ligne") == null)
            {
                lignePerso.NumeroQuestion = ligne.NumeroQuestion;
                lignePerso.Ligne = ligne.Ligne;
                lignePerso.listeChoixReponses = new List<ChoixDeReponse>();
            }
            //Si des parties de phrases ont été envoyé avant
            else
            {
                lignePerso = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
                //Ajouter le reste de la ligne a la phrase
                lignePerso.Ligne = lignePerso.Ligne + ligne.Ligne;
            }
            //this.HttpContext.Session.SetString("Ligne", JsonConvert.SerializeObject(lignePerso));
            //Ajouter cette ligne a la session exercice
            //Ajouter cette ligne a la liste de la session de l'exercice en cours
            InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            //exercice.listeLignes.Add(ligne.NumeroQuestion, ligne);
            exercice.listeLignes.Add(lignePerso);
            //Mettre a jour la session
            this.HttpContext.Session.SetString("Exercice", JsonConvert.SerializeObject(exercice));
            //Mettre fin a la sesisond e ligne
            this.HttpContext.Session.Remove("Ligne");
        }

        [HttpPost]
        public void AjoutChoixReponse([FromBody][Bind("ChoixDeReponse1,Response,NoOrdre")]List<ChoixDeReponseTest> choix)
        {
            //Insérer la liste au contexte
            LignePerso lignePerso = JsonConvert.DeserializeObject<LignePerso>(this.HttpContext.Session.GetString("Ligne"));
            InsertionExercice insertion = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            List<ChoixDeReponse> listeChoix = new List<ChoixDeReponse>();
            foreach (ChoixDeReponseTest choixTest in choix)
            {
                listeChoix.Add(new ChoixDeReponse()
                {
                    ChoixDeReponse1 = choixTest.ChoixDeReponse1,
                    Response = choixTest.Response,
                    NoOrdre = choixTest.NoOrdre,
                    //IdLigne = this.provider.LigneTestInteractif.ToList().Find(x => x.Idexercice == insertion.exercice.Idexercice && x.NumeroQuestion == lignePerso.NumeroQuestion).IdLigne
                });
            }
            //Associer la  liste de choix de reponse a la liste en cours
            lignePerso.listeChoixReponses = listeChoix;
            //Mettre la session a jours
            this.HttpContext.Session.SetString("Ligne", JsonConvert.SerializeObject(lignePerso));
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
            //Mettre fin a la sesisond e ligne
            this.HttpContext.Session.Remove("Ligne");
        }


        [HttpPost]
        public ActionResult EnvoyerExercice()
        {
            //Envoyer le contenue de l'insertion vers la BD
            InsertionExercice Insertionexercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));

            //Pour ce faire on créer un objet exercices et on l'on remplit a partir de notre view model insertionExercice
            Exercice ExercicesAuComplet = new Exercice()
            {
                AdresseCourriel = Insertionexercice.exercice.AdresseCourriel,
                NomExercices = Insertionexercice.exercice.NomExercices,
                ExercicesInt = JsonConvert.SerializeObject(Insertionexercice.listeLignes),
                DateInsertion = DateTime.Now,
                TypeExercice = Insertionexercice.exercice.TypeExercice,
                IdDifficulte = Insertionexercice.exercice.IdDifficulte,
                IdCateg = Insertionexercice.exercice.IdCateg,

            };


            //ajouter le lien à la base de données
            string query = @"UPDATE Exercice SET ExercicesInt ='" + ExercicesAuComplet.ExercicesInt + "' WHERE NomExercices = '" + ExercicesAuComplet.NomExercices + "'";
            SqlCommand commande = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            SqlDataReader reader = commande.ExecuteReader();
            sqlConnection.Close();
            //Tout annuler les sessions concernés
            this.HttpContext.Session.Remove("Ligne");
            this.HttpContext.Session.Remove("Exercice");
            //return RedirectToAction(nameof(ListeExercice));
            return Json(Url.Action("ListeExercice", "Exercice"));
        }

        [HttpPost]
        public bool VerifierNumero(string numero)
        {
            try
            {
                int num = int.Parse(numero);
                bool disponible = true;
                //Voir si la session existe
                if (this.HttpContext.Session.GetString("Exercice") != null)
                {
                    //Associer la liste
                    InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
                    //si le numero existe deja dans la sortedList
                    //if (exercice.listeLignes.ContainsKey(numero))
                    if (exercice.listeLignes.Any(x => x.NumeroQuestion == num))
                    {
                        disponible = false;
                    }
                }
                //Si la session n'existe pas, c'ests ur que le numeroe st disponible
                return disponible;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost]
        public void IndiquerUneSuite()
        {
            //Insérer dans la session de phrase en cours
            this.HttpContext.Session.SetString("PhraseASuivre", "Actif");
        }

        [HttpPost]
        public void AnnulerUneSuite()
        {
            //Insérer dans la session de phrase en cours
            this.HttpContext.Session.SetString("PhraseASuivre", "Innactif");
        }


        [HttpGet]
        public ActionResult AfficherExercice(int id)
        {
            Exercice exercice = this.provider.Exercice.ToList().Find(x => x.Idexercice == id);
            List<LignePerso> list = JsonConvert.DeserializeObject<List<LignePerso>>(exercice.ExercicesInt);
            ViewBag.exercice = exercice;
            ViewBag.lignesexercice = list;
            return View();
        }

        [HttpPost]
        public ActionResult ValiderExercice()
        {
            return View();
        }

        //[HttpPost]
        //public List<bool> Correction(List<string> ListReponse)
        //{
        //    //int idExercice = 4;//int.Parse(this.HttpContext.Session.GetString("Exercice"));
        //    ////Liste comparative des bonnes reponses
        //    //List<bool> listeResultat = new List<bool>();
        //    ////Liste des bonnes reponses
        //    //List<string> listeBonneReponse = this.provider.Exercice.ToList().Find(x=>x.ExercicesInt.)
        //    //int i = 0;
        //    //return new List<bool>();
        //}
    }
}