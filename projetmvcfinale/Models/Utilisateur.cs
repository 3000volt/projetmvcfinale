using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Utilisateur
    {
        public Utilisateur()
        {
            Commentaires = new HashSet<Commentaires>();
            Exercice = new HashSet<Exercice>();
            NoteDeCours = new HashSet<NoteDeCours>();
        }
       [Display(Name ="Courriel")]   
        public string AdresseCourriel { get; set; }
        public string Nom { get; set; }
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }
        [Display(Name = "Date d'enregistrement")]
        public DateTime RegistrerDate { get; set; }


        public ICollection<Commentaires> Commentaires { get; set; }
        [Display(Name = "")]
        public ICollection<Exercice> Exercice { get; set; }
        [Display(Name = "Note de cours")]
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
    }
}
