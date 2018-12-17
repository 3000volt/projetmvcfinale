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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace projetmvcfinale.Controllers
{
    public class ExerciceController : Controller
    {
        //Propriétés du controlleur
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

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Pour permettre au ViewBag contenant les categories d'etre accessible en tout temps    
            base.OnActionExecuted(context);
            ViewBag.Categories = this.provider.Categorie.ToList();
            ViewBag.Notes = this.provider.NoteDeCours.ToList();
            ViewBag.NiveauDif = this.provider.Niveau.ToList();
            ViewBag.souscatégorie = this.provider.SousCategorie.ToList();
            //Merci https://stackoverflow.com/questions/40330391/set-viewbag-property-in-the-constructor-of-a-asp-net-mvc-core-controller//
        }

        /// <summary>
        /// Afficher la liste d'exercices
        /// </summary>
        /// <returns></returns>
        public IActionResult ListeExercice(string search)
        {
            try
            {
                ViewBag.listecorriger = this.provider.Corrige.ToList();
                ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");
                ViewBag.IdDocument = new SelectList(this.provider.NoteDeCours, "IdDocument", "NomNote");
                ViewBag.model = new AssocierDoc();
                return View(this.provider.Exercice.Where(x => x.NomExercices.StartsWith(search) || search == null).ToList());
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        public IActionResult ListeExercice2(string diff, string categ, bool interactif)
        {
            try
            {
                //Chercher la liste en consequence de la demande
                List<Exercice> liste = new List<Exercice>();
                ViewBag.listecorriger = this.provider.Corrige.ToList();
                ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");
                ViewBag.Idcorrige = new SelectList(this.provider.Corrige, "Idcorrige", "CorrigeDocNom");
                ViewBag.model = new AssocierDoc();
                int categorie = this.provider.Categorie.ToList().Find(x => x.NomCategorie == categ).IdCateg;
                int difficulte = this.provider.Niveau.ToList().Find(x => x.NiveauDifficulte == diff).IdDifficulte;
                if (interactif == false)
                {
                    liste = this.provider.Exercice.ToList().FindAll(x => x.IdCateg == categorie && x.IdDifficulte == difficulte && x.TypeExercice == "Normal");
                }
                else if (interactif == true)
                {
                    liste = this.provider.Exercice.ToList().FindAll(x => x.IdCateg == categorie && x.IdDifficulte == difficulte && x.TypeExercice == "Interactif");
                }
                return View("ListeExercice", liste);
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        /// <summary>
        /// Affiche la vue pour ajouter un exercice
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjoutExercice()
        {
            //source:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1, https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core
            //https://stackoverflow.com/questions/20380994/convert-list-to-ienumerableselectlistitem
            try
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

                return View();
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }


        }

        /// <summary>
        /// Ajouter un exercice
        /// </summary>
        /// <param name="exercice"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjoutExercice(ExerciceVM exerciceVM)
        {
            try
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
                    string test = exercice.AdresseCourriel;

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

                    //Ajouter au contexte
                    provider.Add(exercice);
                    await provider.SaveChangesAsync();
                    HttpContext.Session.SetString("exerciceAjouter", JsonConvert.SerializeObject(exercice));//pour aller le chercher pour l'upload
                    //Envoyer vers la vue pour continuer la creation selon le type
                    return View("CompleterCreation", exerciceVM);
                }
                return BadRequest("Erreur dans l'insertion de l'exercice");
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }


        /// <summary>
        /// Afficher la vue pour téléverser un fichier d'exercice
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadExercice(IFormFile Lien)
        {
            // try
            // {
            bool pfdOuWord = false;
            //Voir si le document est un pdf ou word
            Regex reg = new Regex("\\.pdf$|\\.docx$|\\.doc$");
            Match match = reg.Match(Lien.FileName);
            if (match.Success)
            {
                pfdOuWord = true;
            }

            Exercice ex = JsonConvert.DeserializeObject<Exercice>(this.HttpContext.Session.GetString("exerciceAjouter"));

            if (pfdOuWord == true)
            {
                if (Lien == null || Lien.Length == 0)
                    return Content("Aucun fichier sélectionné");

                var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", Lien.FileName);
                //ajouter le lien à la base de données
                int Idexercice = ex.Idexercice;

                string format = Lien.FileName.Substring(Lien.FileName.Length - 4);
                if (format == "docx")
                {
                    format = ".docx";
                }
                //https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c
                using (var stream = new FileStream(chemin, FileMode.Create))
                {
                    await Lien.CopyToAsync(stream);
                }
                //Change le nom du document
                System.IO.File.Move(chemin, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", ex.Idexercice.ToString() + format));
                //Relier le nouveau lien a l'exercice
                ex.Lien = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", ex.Idexercice.ToString() + format);
                //Modifier la BD
                this.provider.Update(ex);
                await provider.SaveChangesAsync();
                return RedirectToAction(nameof(ListeExercice));
            }

            ViewBag.Nom = ex.NomExercices;
            ViewBag.pdf_Word = "Avertissement";
            List<Exercice> liste = this.provider.Exercice.ToList();
            return View(nameof(ListeExercice), liste);
            //  }
            // catch(Exception e)
            // {
            //     return View("\\Views\\Shared\\page_erreur.cshtml");
            //  }

        }

        public async Task<IActionResult> ModifierExercice(int id)
        {
            if (ModelState.IsValid)
            {
                if (id.ToString() == null)
                    return View("\\Views\\Shared\\page_erreur.cshtml");

                Exercice ex = this.provider.Exercice.ToList().Find(x => x.Idexercice == id);

                if (ex == null)
                    return View("\\Views\\Shared\\page_erreur.cshtml");

                //envoyer a la bonne vue selon le type d'exercice
                if (ex.TypeExercice == "Interactif")
                {
                    //TODO !!!
                    InsertionExercice exerciceModifier = new InsertionExercice()
                    {
                        exercice = ex,
                        listeLignes = JsonConvert.DeserializeObject<List<LignePerso>>(ex.ExercicesInt)
                    };



                    return View("ModifierInteractif", exerciceModifier);
                }
                else
                {
                    //transférer en ViewModel
                    ExerciceVM exercice = new ExerciceVM()
                    {
                        IdExercice = ex.Idexercice,
                        NomExercices = ex.NomExercices,
                        IdCateg = ex.IdCateg,
                        IdDifficulte = ex.IdDifficulte,
                        TypeExercice = ex.TypeExercice,
                    };
                    //conserver le lien
                    this.HttpContext.Session.SetString("Lien", ex.Lien);

                    ViewBag.Niveau = new SelectList(this.provider.Niveau, "IdDifficulte", "NiveauDifficulte");
                    ViewBag.Categorie = new SelectList(this.provider.Categorie, "IdCateg", "NomCategorie");

                    return View("ModifierExercice", exercice);
                }
            }
            return BadRequest();

        }
        /// <summary>
        /// Modifier un exercice de type normale
        /// </summary>
        /// <param name="exerciceVM"></param>
        /// <returns></returns>
        public async Task<IActionResult> ModifierExerciceNormal(ExerciceVM exerciceVM)
        {
            if (ModelState.IsValid)
            {
                if (exerciceVM == null)
                    return NotFound();

                Exercice ex = await provider.Exercice.FindAsync(exerciceVM.IdExercice);

                ex.NomExercices = exerciceVM.NomExercices;
                ex.IdCateg = exerciceVM.IdCateg;
                ex.IdDifficulte = exerciceVM.IdDifficulte;

                //vérifier si le lien est null ou non
                if (exerciceVM.Lien != null)
                {
                    ex.Lien = exerciceVM.Lien.FileName;

                    //changer le document associé
                    if (exerciceVM.Lien != null || exerciceVM.Lien.Length != 0)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", this.HttpContext.Session.GetString("Lien"));
                        string vieuxChemin = path;
                        //supprimer le vieux document
                        if (System.IO.File.Exists(vieuxChemin))
                        {
                            System.IO.File.Delete(vieuxChemin);
                        }
                        //nouveau lien 
                        var nouveauChemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", exerciceVM.Lien.FileName);
                        //inserer le nouveau document
                        using (var stream = new FileStream(nouveauChemin, FileMode.Create))
                        {
                            await exerciceVM.Lien.CopyToAsync(stream);
                        }
                    }
                }

                provider.Exercice.Update(ex);
                await provider.SaveChangesAsync();
                return RedirectToAction(nameof(ListeExercice));

            }
            return BadRequest();
        }


        /// <summary>
        /// Affiche la vue pour supprimer un exercice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult SupprimerExercice(int id)
        {
            if (id.ToString() == null)
                return View("\\Views\\Shared\\page_erreur.cshtml");

            //aller chercher le cours dans le contexte
            Exercice ex = this.provider.Exercice.ToList().Find(x => x.Idexercice == id);

            //vérifier si le cours est null
            if (ex == null)
                return View("\\Views\\Shared\\page_erreur.cshtml");
            //Mettre le lien du doc dans la session s'il s'agit d'un exercie
            //S'il s'agit d'un exercie normal
            if (ex.TypeExercice == "Normal")
            {
                this.HttpContext.Session.SetString("Link", ex.Lien);

            }
            return View(ex);
        }
        /// <summary>
        /// Supprimer un exercice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SupprimerExercicePost(int id)
        {
            try
            {
                Exercice exercice = await provider.Exercice.FindAsync(Convert.ToInt32(id));
                provider.Exercice.Remove(exercice);
                //Supprimer le vieu document
                if (this.provider.Exercice.ToList().Find(x => x.Idexercice == exercice.Idexercice).TypeExercice == "Normal")
                {
                    var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", this.HttpContext.Session.GetString("Link"));
                    string fullPath = chemin;
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    //https://stackoverflow.com/questions/22650740/asp-net-mvc-5-delete-file-from-server
                }

                await provider.SaveChangesAsync();
                return RedirectToAction(nameof(ListeExercice));
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void CreationLigne([FromBody][Bind("NumeroQuestion,Ligne,ChoixDeReponse1,Response,NoOrdre")]LignePerso ligne)
        {
            try
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
            catch (Exception e)
            {
                Redirect("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void TerminerLigne([FromBody][Bind("NumeroQuestion,Ligne")]LignePerso ligne)
        {
            try
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
            catch (Exception e)
            {
                Redirect("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void AjoutChoixReponse([FromBody][Bind("ChoixDeReponse1,Response,NoOrdre")]List<ChoixDeReponseTest> choix)
        {
            try
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
            catch (Exception e)
            {
                Redirect("\\Views\\Shared\\page_erreur.cshtml");
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void TerminerPhrase()
        {
            try
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
            catch (Exception e)
            {
                Redirect("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EnvoyerExercice()
        {
            try
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
                if (ExercicesAuComplet.ExercicesInt.Contains("'"))
                {
                    //int index = ExercicesAuComplet.ExercicesInt.IndexOf("'");
                    //ExercicesAuComplet.ExercicesInt = ExercicesAuComplet.ExercicesInt.Insert(index, "'");
                    ExercicesAuComplet.ExercicesInt = ExercicesAuComplet.ExercicesInt.Replace("'", "''");
                }
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
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }




        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void IndiquerUneSuite()
        {
            //Insérer dans la session de phrase en cours
            this.HttpContext.Session.SetString("PhraseASuivre", "Actif");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void AnnulerUneSuite()
        {
            //Insérer dans la session de phrase en cours
            this.HttpContext.Session.SetString("PhraseASuivre", "Innactif");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AfficherExercice(int id)
        {
            try
            {
                Exercice exercice = this.provider.Exercice.ToList().Find(x => x.Idexercice == id);
                List<LignePerso> list = JsonConvert.DeserializeObject<List<LignePerso>>(exercice.ExercicesInt);
                ViewBag.exercice = exercice;
                ViewBag.lignesexercice = list;
                this.HttpContext.Session.SetString("ExerciceAffiche", JsonConvert.SerializeObject(exercice));
                return View();
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

        //page d'erreur a mettre
        [AllowAnonymous]
        [HttpPost]
        public List<bool> Correction(List<string> ListReponse)
        {
            Exercice exercice = JsonConvert.DeserializeObject<Exercice>(this.HttpContext.Session.GetString("ExerciceAffiche"));
            //Liste comparative des bonnes reponses
            List<string> listeReponse = new List<string>();
            //Liste de validation
            List<bool> listeResultat = new List<bool>();
            //Liste des bonnes reponses
            List<LignePerso> listePhrase = JsonConvert.DeserializeObject<List<LignePerso>>(exercice.ExercicesInt);
            foreach (LignePerso l in listePhrase)
            {
                //Pour chaque choix de reponse dans la question
                foreach (ChoixDeReponse c in l.listeChoixReponses)
                {
                    //S'il s'agit d'une bonne réponse
                    if (c.Response == true)
                    {
                        //Ajouter a la liste de réponse
                        listeReponse.Add(c.ChoixDeReponse1);
                    }
                }
            }
            //Comparer les réponses de l'utilisateur au corrigé
            int compteur = 0;
            foreach (string s in ListReponse)
            {
                //Si la réponse transmise est la même que la bonne réponse
                if (s == listeReponse[compteur])
                {
                    //Mettre vrai dans le corrigé
                    listeResultat.Add(true);
                }
                //s'il s'agit d'une mauvaise réponse, envoyer une erreur
                else
                {
                    //listeResultat.Add(compteur, false);
                    listeResultat.Add(false);
                }
                //Ajouter au compteur
                compteur++;
            }
            return listeResultat;


        }

        [HttpPost]
        public List<int> ListeNumero()
        {

            InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
            List<int> listeNumero = new List<int>();

            foreach (LignePerso s in exercice.listeLignes)
            {
                listeNumero.Add(s.NumeroQuestion);
            }

            return listeNumero;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public void SupprimerLigne(int Ligne)
        {
            try
            {
                //Trouver l,exercice concerné
                InsertionExercice exercice = JsonConvert.DeserializeObject<InsertionExercice>(this.HttpContext.Session.GetString("Exercice"));
                //Retirer la ligne concerné
                LignePerso ligne = exercice.listeLignes.Find(x => x.NumeroQuestion == Ligne);
                //retirer la ligne en question
                exercice.listeLignes.Remove(ligne);
                //Mettre la session à jour
                this.HttpContext.Session.SetString("Exercice", JsonConvert.SerializeObject(exercice));
            }
            catch (Exception e)
            {
                Redirect("\\Views\\Shared\\page_erreur.cshtml");
            }

        }


        /// <summary>
        /// Associer un document a l'exercice
        /// </summary>
        /// <param name="associer"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AssocierDocument(AssocierDoc associer)
        {
            try
            {
                //updater manuellement dans la BD l'id du corrigé associé
                string query = @"UPDATE Exercice SET IdDocument = '" + associer.IdDocument + "' WHERE IdExercice = '" + associer.Idexercice + "'";
                SqlCommand commande = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                SqlDataReader reader = commande.ExecuteReader();
                sqlConnection.Close();

                return RedirectToAction(nameof(ListeExercice));
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }

    }
}