using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Exercice
    {
        public Exercice()
        {
            Corrige = new HashSet<Corrige>();
        }
        [Display(Name = "Numéro")]
        public int Idexercice { get; set; }
        [Display(Name = "Titre")]
        public string NomExercices { get; set; }
        public string Lien { get; set; }
        [Display(Name = "")]//pas sur c'est quoi celui la
        public string ExercicesInt { get; set; }
        [Display(Name = "Date d'insertion")]
        public DateTime DateInsertion { get; set; }
        [Display(Name = "Type")]
        public string TypeExercice { get; set; }
        [Display(Name = "Courriel")]
        public string AdresseCourriel { get; set; }
        [Display(Name = "Difficulté")]
        public int IdDifficulte { get; set; }
        [Display(Name = "Corrigé")]
        public int? Idcorrige { get; set; }
        [Display(Name = "Document")]
        public int? IdDocument { get; set; }
        [Display(Name = "Catégorie")]
        public int IdCateg { get; set; }

        public Utilisateur AdresseCourrielNavigation { get; set; }
        public Categorie IdCategNavigation { get; set; }
        public Niveau IdDifficulteNavigation { get; set; }
        public NoteDeCours IdDocumentNavigation { get; set; }
        public Corrige IdcorrigeNavigation { get; set; }
        public ICollection<Corrige> Corrige { get; set; }
    }
}
