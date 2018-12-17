using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projetmvcfinale.Models;

namespace projetmvcfinale.Controllers
{
    public class NoteDeCoursController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;

        public NoteDeCoursController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Pour permettre au ViewBag contenantn les categores d'etre accessible en tout temps    
            base.OnActionExecuted(context);
            ViewBag.Categories = this.provider.Categorie.ToList();
            ViewBag.Notes = this.provider.NoteDeCours.ToList();
            ViewBag.NiveauDif = this.provider.Niveau.ToList();
            ViewBag.souscatégorie = this.provider.SousCategorie.ToList();
            //Merci https://stackoverflow.com/questions/40330391/set-viewbag-property-in-the-constructor-of-a-asp-net-mvc-core-controller
        }

        public IActionResult ListeNoteDeCours(string search)
        {
            //    List<NoteDeCours> listeNote = this.provider.NoteDeCours.ToList();
            //    return View(listeNote);
            return View(this.provider.NoteDeCours.Where(x => x.IdSousCategorieNavigation.NomSousCategorie.StartsWith(search) || x.IdCategNavigation.NomCategorie.StartsWith(search) || search == null).ToList());
        }
        /// <summary>
        /// Afficher la vue pour ajouter une note
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjouterNote()
        {
            ViewBag.IdCateg = new SelectList(this.provider.Categorie.ToList(), "IdCateg", "NomCategorie");
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjouterNote([Bind("NomNote,IdCateg,SousCategorie,Lien")] NotesViewModel noteVM)
        {
            bool pfdOuWord = true;
            bool lienAjoute = true;

            if (ModelState.IsValid)
            {
                if (noteVM.Lien == null)
                {
                    lienAjoute = false;
                }
                else
                {
                    pfdOuWord = false;
                    //Voir si le document est un pdf ou word
                    Regex reg = new Regex("\\.pdf$|\\.docx$|\\.doc$");
                    Match match = reg.Match(noteVM.Lien.FileName);
                    if (match.Success)
                    {
                        pfdOuWord = true;
                    }
                }

                if (pfdOuWord == true)
                {
                    //Transferer en note
                    NoteDeCours note;
                    if (noteVM.Lien != null)
                    {
                        note = new NoteDeCours()
                        {
                            NomNote = noteVM.NomNote,
                            IdCateg = noteVM.IdCateg,
                            IdSousCategorie = this.provider.SousCategorie.ToList().Find(x => x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie,
                            Lien = noteVM.Lien.FileName,
                            AdresseCourriel = this.HttpContext.User.Identity.Name,
                            DateInsertion = DateTime.Now

                        };
                    }
                    else
                    {
                        note = new NoteDeCours()
                        {
                            NomNote = noteVM.NomNote,
                            IdCateg = noteVM.IdCateg,
                            IdSousCategorie = this.provider.SousCategorie.ToList().Find(x => x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie,
                            AdresseCourriel = this.HttpContext.User.Identity.Name,
                            Lien = "",
                            DateInsertion = DateTime.Now

                        };
                    }

                    //note.DateInsertion = DateTime.Today;
                    //note.AdresseCourriel = JsonConvert.DeserializeObject<Utilisateur>(this.HttpContext.Session.GetString("user")).AdresseCourriel;
                    provider.Add(note);
                    await provider.SaveChangesAsync();
                    //HttpContext.Session.SetString("NoteDeCours", JsonConvert.SerializeObject(note));//pour aller le chercher pour l'upload
                    //Insérer dans la BD le document
                    if (lienAjoute == true)
                    {
                        var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.Lien);
                        //Prendre la find du doc(.pdf / .doc / docx)
                        string format = note.Lien.Substring(note.Lien.Length - 4);
                        if (format == "docx")
                        {
                            format = ".docx";
                        }
                        //https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c
                        using (var stream = new FileStream(chemin, FileMode.Create))
                        {
                            await noteVM.Lien.CopyToAsync(stream);
                        }
                        //Change le nom du document
                        System.IO.File.Move(chemin, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.IdDocument.ToString() + format));
                        //ajouter le lien à la base de données
                        note.Lien = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.IdDocument.ToString() + format);
                        await provider.SaveChangesAsync();
                    }


                    return RedirectToAction(nameof(ListeNoteDeCours));
                }
                ViewBag.pdf_Word = "Avertissement";

            }
            ViewBag.IdCateg = new SelectList(this.provider.Categorie.ToList(), "IdCateg", "NomCategorie");
            return View(noteVM);
        }

        /// <summary>
        /// Modifier une note éxistante
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ModifierNote(int id)
        {
            //Retirer les bonnes notes de cours
            NoteDeCours noteDeCours = this.provider.NoteDeCours.ToList().Find(x => x.IdDocument == id);
            //Version view Model
            NotesViewModel noteVM = new NotesViewModel()
            {
                NomNote = noteDeCours.NomNote,
                //Lien = noteDeCours.Lien,
                IdCateg = noteDeCours.IdCateg,
                SousCategorie = this.provider.SousCategorie.ToList().Find(x => x.IdSousCategorie == noteDeCours.IdSousCategorie).NomSousCategorie
            };
            //Insérer le ID dans une session
            this.HttpContext.Session.SetString("IdNotes", id.ToString());//TODO: Bien le gérer
                                                                         //Le lien actuel dans une session egalement
                this.HttpContext.Session.SetString("Link", noteDeCours.Lien);
            this.HttpContext.Session.SetString("Lien", JsonConvert.SerializeObject(noteDeCours.Lien));
            //Viewbag contenant le lien du document
            ViewBag.Link = noteDeCours.Lien;
            //Retourenr la vue permettant de modifier
            ViewBag.IdCateg = new SelectList(this.provider.Categorie.ToList(), "IdCateg", "NomCategorie");
            this.HttpContext.Session.SetString("noteVm", JsonConvert.SerializeObject(noteVM));
            return View(noteVM);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ModifierNote(NotesViewModel noteVM)
        {
            bool changement = true;
            bool pfdOuWord = false;
            //Récupréer le lien si aucun lien n'a été indiqué
            if (noteVM.Lien == null)
            {
                changement = false;
            }
            if (ModelState.IsValid)
            {
                if (changement == true)
                {
                    //Voir si le document est un pdf ou word
                    Regex reg = new Regex("\\.pdf$|\\.docx$|\\.doc$");
                    Match match = reg.Match(noteVM.Lien.FileName);
                    if (match.Success)
                    {
                        pfdOuWord = true;
                    }
                }

                if (pfdOuWord == true || changement == false)
                {
                    //Trouver la note de cours correspondant
                    //Récupérer le ID en du note de cours
                    int id = int.Parse(this.HttpContext.Session.GetString("IdNotes"));
                    NoteDeCours noteDeCours = this.provider.NoteDeCours.ToList().Find(x => x.IdDocument == id);
                    //Changer les valeurs
                    noteDeCours.IdCateg = noteVM.IdCateg;
                    noteDeCours.NomNote = noteVM.NomNote;
                    noteDeCours.IdSousCategorie = this.provider.SousCategorie.ToList().Find(x => x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie;

                    //S'il y a eu des modifications dans les liens
                    if (changement == true)
                    {
                        noteDeCours.Lien = noteVM.Lien.FileName;
                        //Supprimer le vieu document
                        var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", this.HttpContext.Session.GetString("Link"));

                        if (System.IO.File.Exists(chemin))
                        {
                            System.IO.File.Delete(chemin);
                        }
                        //https://stackoverflow.com/questions/22650740/asp-net-mvc-5-delete-file-from-server
                        var nouveauChemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", noteDeCours.Lien);
                        //Prendre la find du doc(.pdf / .doc / docx)
                        string format = noteDeCours.Lien.Substring(noteDeCours.Lien.Length - 4);
                        if (format == "docx")
                        {
                            format = ".docx";
                        }
                        //https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c

                        using (var stream = new FileStream(nouveauChemin, FileMode.Create))
                        {
                            await noteVM.Lien.CopyToAsync(stream);
                        }
                        //Change le nom du document
                        System.IO.File.Move(nouveauChemin, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", noteDeCours.IdDocument.ToString() + format));
                        //ajouter le lien à la base de données
                        noteDeCours.Lien = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", noteDeCours.IdDocument.ToString() + format);
                        this.provider.Update(noteDeCours);
                        await provider.SaveChangesAsync();
                    }

                    //Sauvegarder dans la bd
                    provider.Update(noteDeCours);
                    await provider.SaveChangesAsync();
                    return RedirectToAction(nameof(ListeNoteDeCours));
                }
                ViewBag.pdf_Word = "Avertissement";
            }
            ViewBag.IdCateg = new SelectList(this.provider.Categorie.ToList(), "IdCateg", "NomCategorie");
            NotesViewModel noteEchec = JsonConvert.DeserializeObject<NotesViewModel>(this.HttpContext.Session.GetString("noteVm"));
            return View(noteEchec);
        }

        /// <summary>
        /// afficher la note a supprimer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult SupprimerNote(int id)
        {
            //Retirer les bonnes notes de cours
            NoteDeCours noteDeCours = this.provider.NoteDeCours.ToList().Find(x => x.IdDocument == id);
            //Version view Model
            NotesViewModel noteVM = new NotesViewModel()
            {
                NomNote = noteDeCours.NomNote,
                //Lien = noteDeCours.Lien,
                IdCateg = noteDeCours.IdCateg,
                SousCategorie = this.provider.SousCategorie.ToList().Find(x => x.IdSousCategorie == noteDeCours.IdSousCategorie).NomSousCategorie
            };
            //Insérer le ID dans une session
            this.HttpContext.Session.SetString("IdNotes", id.ToString());//TODO: Bien le gérer
                                                                         //Le lien actuel dans une session egalement
            this.HttpContext.Session.SetString("Link", noteDeCours.Lien);
            //Viewbag contenant le lien du document
            ViewBag.Link = noteDeCours.Lien;
            //Retourenr la vue permettant de modifier
            return View(noteVM);
        }
        /// <summary>
        /// Supprimer la note ainsi que son document
        /// </summary>
        /// <param name="noteVM"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SupprimerNote(NotesViewModel noteVM)
        {
            //Trouver la note de cours correspondant
            //Récupérer le ID en du note de cours
            int id = int.Parse(this.HttpContext.Session.GetString("IdNotes"));
            NoteDeCours noteDeCours = this.provider.NoteDeCours.ToList().Find(x => x.IdDocument == id);
            //Supprimer le vieu document
            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", this.HttpContext.Session.GetString("Link"));
            string fullPath = chemin;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            //https://stackoverflow.com/questions/22650740/asp-net-mvc-5-delete-file-from-server
            //Sauvegarder dans la bd
            provider.Remove(noteDeCours);
            await provider.SaveChangesAsync();
            return RedirectToAction(nameof(ListeNoteDeCours));
        }

        /// <summary>
        /// Retourne la vue pour ajouter un document
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UploadNote()
        {
            return View();
        }
        /// <summary>
        /// téléverser le document
        /// </summary>
        /// <param name="Lien"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadNote(IFormFile Lien)
        {
            //Trouver la bonne note
            NoteDeCours note = JsonConvert.DeserializeObject<NoteDeCours>(this.HttpContext.Session.GetString("NoteDeCours"));

            //vérifier si le lien est null
            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

            //créer le chemin
            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Exercices", Lien.FileName);

            note.Lien = chemin;
            provider.NoteDeCours.Update(note);
            await provider.SaveChangesAsync();

            using (var stream = new FileStream(chemin, FileMode.Create))
            {
                await Lien.CopyToAsync(stream);
            }

            return Ok("Fichier téléversé avec succès!");
        }
        /// <summary>
        /// Retrier une sous-catégorie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public List<string> RetirerSousCateg(int id)
        {
            List<string> sousCategorie = new List<string>();
            //Trouver tout les
            sousCategorie = this.provider.SousCategorie.ToList().FindAll(x => x.IdCateg == id).Select(x => x.NomSousCategorie).ToList();
            //Retourner la liste
            return sousCategorie;
        }

        /// <summary>
        /// Créer un document pdf
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult creerpdf()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreerSousCategorie([FromBody][Bind("NomSousCategorie,idCateg")] SousCategorie sousCategorie)
        {
            //Voir si la donnée est valide
            if (ModelState.IsValid)
            {
                //Ajouter a la bd
                this.provider.Add(sousCategorie);
                this.provider.SaveChanges();
                return Ok("élément modifié avec succès");
            }
            return BadRequest("Erreur de modification");
        }
    }
}