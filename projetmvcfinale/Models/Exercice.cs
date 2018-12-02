using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Exercice
    {
        //public Exercice()
        //{
        //    //Corrige = new HashSet<Corrige>();
        //    //LigneTestInteractif = new HashSet<LigneTestInteractif>();
        //}

        [Display(Name = " Numéro d'identifiaction")]
        public int Idexercice { get; set; }
        [Display(Name = "Titre")]
        public string NomExercices { get; set; }
        public string Lien { get; set; }
        [Display(Name = "Date d'ajout")]
        public DateTime DateInsertion { get; set; }
        [Display(Name = "Type")]
        public string TypeExercice { get; set; }
        [Display(Name = "Créateur")]
        public string AdresseCourriel { get; set; }
        [Display(Name = "Niveau de difficulté")]
        public int IdDifficulte { get; set; }
        [Display(Name = "Corrigé")]
        public int? Idcorrige { get; set; }
        [Display(Name = "Document")]
        public int? IdDocument { get; set; }
        [Display(Name = "Catégorie")]
        public int IdCateg { get; set; }

        [Display(Name = "Créateur")]
        public Utilisateur AdresseCourrielNavigation { get; set; }
        [Display(Name = "Catégorie")]
        public Categorie IdCategNavigation { get; set; }
        [Display(Name = "Niveau de difficulté")]
        public Niveau IdDifficulteNavigation { get; set; }
        [Display(Name = "Document")]
        public NoteDeCours IdDocumentNavigation { get; set; }
        [Display(Name = "Corrigé")]
        public Corrige IdcorrigeNavigation { get; set; }
        [Display(Name = "")]
        public ICollection<Corrige> Corrige { get; set; }
        [Display(Name = "")]
        public ICollection<LigneTestInteractif> LigneTestInteractif { get; set; }
    }
}
