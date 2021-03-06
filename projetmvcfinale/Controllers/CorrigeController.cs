﻿using System;
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
    public class CorrigeController : Controller
    {
        private readonly ProjetFrancaisContext provider;
        private readonly IConfiguration Configuration;
        public string ConnectionString;
        private SqlConnection sqlConnection;
        private List<Corrige> listeCor;

        //Controlleur
        public CorrigeController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.provider = new ProjetFrancaisContext(this.Configuration.GetConnectionString("DefaultConnection"));
            this.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            this.sqlConnection = new SqlConnection(this.ConnectionString);
            this.listeCor = this.provider.Corrige.ToList();
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
        public IActionResult ListeCorrige(string search)
        {
            //Retourne la liste des corrigés
            return View(this.provider.Corrige.Where(x => x.CorrigeDocNom.StartsWith(search) || search == null).ToList());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AjouterCorrige()
        {
            //Retourne le selectList de tout les exercices
            ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");
            //Retourne la vue pour ajouter un corrigé
            return View();
        }
        /// <summary>
        /// Ajouter un corrigé dans la bd
        /// </summary>
        /// <param name="corrige"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AjouterCorrige([Bind("CorrigeDocNom,Lien,Idexercice")] CorrigeViewModel corrigeVM)
        {
            try
            {
                //Boolean regardant s'il s'agit d'un exercice pdf ou word
                bool pdfOuWord = true;
                //Vérifie si un lien a été ajouté
                if (corrigeVM.Lien != null)
                {
                    if (ModelState.IsValid)
                    {
                        pdfOuWord = false;
                        //Voir si le document est un pdf ou word
                        Regex reg = new Regex("\\.pdf$|\\.docx$|\\.doc$");
                        Match match = reg.Match(corrigeVM.Lien.FileName);
                        if (match.Success)
                        {
                            //Laisse savoir qu'il s'agit d'un pdf ou word
                            pdfOuWord = true;
                        }
                        //s'il s'agit d'un pdf ou word
                        if (pdfOuWord == true)
                        {
                            //conversion du ViewModel en corrigé
                            Corrige corrige = new Corrige()
                            {
                                CorrigeDocNom = corrigeVM.CorrigeDocNom,
                                Lien = corrigeVM.Lien.FileName,
                                DateInsertion = DateTime.Now,
                                Idexercice = corrigeVM.Idexercice
                            };
                            //ajoute le corrige
                            provider.Add(corrige);
                            await provider.SaveChangesAsync();
                            //associer
                            Exercice ex = this.provider.Exercice.ToList().Find(x => x.Idexercice == corrige.Idexercice);
                            ex.Idcorrige = corrige.Idcorrige;
                            provider.Exercice.Update(ex);
                            await provider.SaveChangesAsync();
                            //Insérer dans la BD le document     
                            var chemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", corrige.Lien);
                            //Prendre la find du doc(.pdf / .doc / docx)
                            string format = corrige.Lien.Substring(corrige.Lien.Length - 4);
                            if (format == "docx")
                            {
                                format = ".docx";
                            }
                            //https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c
                            using (var stream = new FileStream(chemin, FileMode.Create))
                            {
                                await corrigeVM.Lien.CopyToAsync(stream);
                            }
                            //Change le nom du document
                            System.IO.File.Move(chemin, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", corrige.Idcorrige.ToString() + format));
                            //ajouter le lien à la base de données
                            corrige.Lien = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\NoteDeCours", corrige.Idcorrige.ToString() + format);
                            await provider.SaveChangesAsync();

                            return RedirectToAction(nameof(ListeCorrige));
                        }
                        //S'il ne s'agit aps de pdf ou word, retourner la view d'ajout
                        ViewBag.pdf_Word = "Avertissement";
                    }
                }
                //Si aucun lien n'a été rajouté, retourner la vue d'ajout et ne pas accepter la requete
                ViewBag.pdf_Word2 = "Avertissement";
                ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");
                return View("AjouterCorrige");
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }
        }

        /// <summary>
        /// Afficher la vue pour modifier un corrige
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> ModifierCorrige(int id)
        {
            try
            {
                //Si le id n est pas valide
                if (id.ToString() == null)
                    return View("\\Views\\Shared\\page_erreur.cshtml");
                //Trouver le corrigé associé
                Corrige cr = await provider.Corrige.FindAsync(id);
                //Si le corrigé n'existe pas
                if (cr == null)
                    return View("\\Views\\Shared\\page_erreur.cshtml");
                //transfer en ViewModel
                CorrigeViewModel corrige = new CorrigeViewModel()
                {
                    idcorrige = cr.Idcorrige,
                    CorrigeDocNom = cr.CorrigeDocNom,
                };
                //lien du document
                this.HttpContext.Session.SetString("Lien", cr.Lien);
                //liste d'exercice existant
                ViewBag.Idexercice = new SelectList(this.provider.Exercice, "Idexercice", "NomExercices");

                return View(corrige);
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }
        /// <summary>
        /// Modifier un corrigé existant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> ModifierCorrige(CorrigeViewModel corrigeVM)
        {
            if (ModelState.IsValid)
            {
                //Si le corrigeVM n'est pas valide
                if (corrigeVM == null)
                    return NotFound();
                //Trouver le corrigé associé
                Corrige cr = await provider.Corrige.FindAsync(corrigeVM.idcorrige);
                //Mettre a jour ses variables envoyés
                cr.CorrigeDocNom = corrigeVM.CorrigeDocNom;
                cr.Idexercice = corrigeVM.Idexercice;

                //vérifier si le lien est existant ou non
                if (corrigeVM.Lien != null)
                {
                    cr.Lien = corrigeVM.Lien.FileName;

                    //changer le document associé
                    if (corrigeVM.Lien != null || corrigeVM.Lien.Length != 0)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", this.HttpContext.Session.GetString("Lien"));
                        string vieuxChemin = path;
                        //supprimer le vieux document
                        if (System.IO.File.Exists(vieuxChemin))
                        {
                            System.IO.File.Delete(vieuxChemin);
                        }
                        //nouveau lien 
                        var nouveauChemin = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Corrige", corrigeVM.Lien.FileName);
                        //inserer le nouveau document
                        using (var stream = new FileStream(nouveauChemin, FileMode.Create))
                        {
                            await corrigeVM.Lien.CopyToAsync(stream);
                        }
                    }
                }
                //Mettre a jour et renvoyer vers la liste des corrigés
                provider.Corrige.Update(cr);
                await provider.SaveChangesAsync();
                return RedirectToAction(nameof(ListeCorrige));
            }
            return View(corrigeVM);
        }
        /// <summary>
        /// Afficher la vue avant de supprimer un corrige
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> SupprimerCorrige(int id)
        {
            //si le id n'ets pa valide
            if (id.ToString() == null)
                return NotFound();

            //aller chercher le corrigé dans le contexte
            Corrige cr = await provider.Corrige.FindAsync(id);

            //vérifier si le corrigé est null
            if (cr == null)
                return NotFound();

            return View(cr);
        }

        /// <summary>
        /// Supprimer un corrige de la liste
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> SupprimerCorrigePost(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Corrige corrige = await provider.Corrige.FindAsync(id);
                    string chemin = corrige.Lien;
                    //trouver l'exercice correspondant & mettre l'id du corrige a null
                    Exercice ex = await provider.Exercice.FindAsync(corrige.Idexercice);

                    //updater manuellement dans la BD l'id du corrigé associé
                    string query = @"UPDATE Exercice SET IdCorrige = Null WHERE IdExercice = '" + ex.Idexercice + "'";
                    SqlCommand commande = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader reader = commande.ExecuteReader();
                    sqlConnection.Close();

                    //supprimer le corrige de la BD
                    provider.Corrige.Remove(corrige);

                    //supprimer le fichier
                    if (System.IO.File.Exists(chemin))
                    {
                        System.IO.File.Delete(chemin);
                    }
                    await provider.SaveChangesAsync();
                    return RedirectToAction(nameof(ListeCorrige));
                }
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }
            catch (Exception e)
            {
                return View("\\Views\\Shared\\page_erreur.cshtml");
            }

        }
    }
}