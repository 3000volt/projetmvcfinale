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
            LigneTestInteractif = new HashSet<LigneTestInteractif>();
        }
        [Display(Name = "Numéro d'exercice")]
        public int Idexercice { get; set; }
        [Display(Name = "Nom de l'exercice")]
        public string NomExercices { get; set; }
        public string Lien { get; set; }
        [Display(Name = "Date d'insertion")]
        public DateTime DateInsertion { get; set; }
        [Display(Name = "Type d'exercice")]
        public string TypeExercice { get; set; }
        [Display(Name = "Courriel")]
        public string AdresseCourriel { get; set; }
        [Display(Name = "Difficulté")]
        public int IdDifficulte { get; set; }
        [Display(Name = "Numéro du corrigé")]
        public int? Idcorrige { get; set; }
        [Display(Name = "Numéro du document")]
        public int? IdDocument { get; set; }
        [Display(Name = "Numéro de catégorie")]
        public int IdCateg { get; set; }

        public Utilisateur AdresseCourrielNavigation { get; set; }
        public Categorie IdCategNavigation { get; set; }
        public Niveau IdDifficulteNavigation { get; set; }
        public NoteDeCours IdDocumentNavigation { get; set; }
        public Corrige IdcorrigeNavigation { get; set; }
        public ICollection<Corrige> Corrige { get; set; }
        public ICollection<LigneTestInteractif> LigneTestInteractif { get; set; }
    }
}
