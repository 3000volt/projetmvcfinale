﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class NoteDeCours
    {
        public NoteDeCours()
        {
            Exercice = new HashSet<Exercice>();
        }
        [Display(Name = "Numéro")]
        public int IdDocument { get; set; }
        [Display(Name = "Titre du document")]
        public string NomNote { get; set; }
        public string Lien { get; set; }
        [Display(Name = "Date d'insertion")]
        public DateTime DateInsertion { get; set; }
        [Display(Name = "Courriel")]
        public string AdresseCourriel { get; set; }
        [Display(Name = "Catégorie")]
        public int IdCateg { get; set; }
        [Display(Name = "Sous-catégorie")]
        public int IdSousCategorie { get; set; }

        [Display(Name = "Adresse Courriel")]
        public Utilisateur AdresseCourrielNavigation { get; set; }
        public Categorie IdCategNavigation { get; set; }
        [Display(Name = "Sous catégorie")]
        public SousCategorie IdSousCategorieNavigation { get; set; }
        public ICollection<Exercice> Exercice { get; set; }
    }
}
