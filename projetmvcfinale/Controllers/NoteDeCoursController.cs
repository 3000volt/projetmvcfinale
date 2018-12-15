﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
    public class NoteDeCoursController : Controller//
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

        public IActionResult ListeNoteDeCours()
        {
            //    List<NoteDeCours> listeNote = this.provider.NoteDeCours.ToList();
            //    return View(listeNote);
            return View(this.provider.NoteDeCours.ToList());
        }

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
            if (ModelState.IsValid)
            {
                //Transferer en note
                NoteDeCours note = new NoteDeCours()
                {
                    NomNote = noteVM.NomNote,
                    IdCateg = noteVM.IdCateg,
                    IdSousCategorie = this.provider.SousCategorie.ToList().Find(x => x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie,
                    Lien = noteVM.Lien.FileName,
                    AdresseCourriel = this.HttpContext.User.Identity.Name,
                    DateInsertion = DateTime.Now

                };
                //note.DateInsertion = DateTime.Today;
                //note.AdresseCourriel = JsonConvert.DeserializeObject<Utilisateur>(this.HttpContext.Session.GetString("user")).AdresseCourriel;
                provider.Add(note);
                await provider.SaveChangesAsync();
                //HttpContext.Session.SetString("NoteDeCours", JsonConvert.SerializeObject(note));//pour aller le chercher pour l'upload
                //Insérer dans la BD le document
                if (note.Lien == null || note.Lien.Length == 0)
                    return Content("Aucun fichier sélectionné");

                var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.Lien);

                //ajouter le lien à la base de données
               // note.Lien = chemin;
                //provider.Exercice.Update(ex);
                

                using (var stream = new FileStream(chemin, FileMode.Create))
                {
                    await noteVM.Lien.CopyToAsync(stream);
                }
                //Change le nom du document
                System.IO.File.Move(chemin, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.IdDocument.ToString() + ".pdf"));
                //ajouter le lien à la base de données
                note.Lien = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", note.IdDocument.ToString() + ".pdf");
                await provider.SaveChangesAsync();
                return RedirectToAction(nameof(ListeNoteDeCours));
            }

            return RedirectToAction(nameof(ListeNoteDeCours));
        }

        public IActionResult InfoNote(int id)
        {

            if (id == null)
                return NotFound();

            return View();
        }

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
            //Viewbag contenant le lien du document
            ViewBag.Link = noteDeCours.Lien;
            //Retourenr la vue permettant de modifier
            return View(noteVM);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ModifierNote(NotesViewModel noteVM)
        {
            //Trouver la note de cours correspondant
            //Récupérer le ID en du note de cours
            int id = int.Parse(this.HttpContext.Session.GetString("IdNotes"));
            NoteDeCours noteDeCours = this.provider.NoteDeCours.ToList().Find(x => x.IdDocument == id);
            //Changer les valeurs
            noteDeCours.IdCateg = noteVM.IdCateg;
            if (noteVM.Lien != null)
            {
                noteDeCours.Lien = noteVM.Lien.FileName;
            }

            noteDeCours.NomNote = noteVM.NomNote;
            noteDeCours.IdSousCategorie = this.provider.SousCategorie.ToList().Find(x => x.NomSousCategorie == noteVM.SousCategorie).IdSousCategorie;
            //Mettre le document a jour
            //S'il y a eu des modifications dans les liens
            if (noteDeCours.Lien == null || noteDeCours.Lien.Length == 0)
            {
            }

            else //(noteDeCours.Lien != null || noteDeCours.Lien.Length != 0)
            {
                //Supprimer le vieu document
                var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", this.HttpContext.Session.GetString("Link"));
                string fullPath = chemin;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                //https://stackoverflow.com/questions/22650740/asp-net-mvc-5-delete-file-from-server
                var nouveauChemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", noteDeCours.Lien);
                //ajouter le lien à la base de données
                noteDeCours.Lien = nouveauChemin;
                //provider.Exercice.Update(ex);
                await provider.SaveChangesAsync();
                using (var stream = new FileStream(nouveauChemin, FileMode.Create))
                {
                    await noteVM.Lien.CopyToAsync(stream);
                }
            }
            //Sauvegarder dans la bd
            provider.Update(noteDeCours);
            await provider.SaveChangesAsync();
            return RedirectToAction(nameof(ListeNoteDeCours));
        }

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
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UploadNote()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lien"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadNote(IFormFile Lien)
        {
            NoteDeCours note = JsonConvert.DeserializeObject<NoteDeCours>(this.HttpContext.Session.GetString("NoteDeCours"));

            if (Lien == null || Lien.Length == 0)
                return Content("Aucun fichier sélectionné");

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

        [HttpPost]
        public List<string> RetirerSousCateg(int id)
        {
            List<string> sousCategorie = new List<string>();
            //Trouver tout les
            sousCategorie = this.provider.SousCategorie.ToList().FindAll(x => x.IdCateg == id).Select(x => x.NomSousCategorie).ToList();
            //Retourner la liste
            return sousCategorie;
        }


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