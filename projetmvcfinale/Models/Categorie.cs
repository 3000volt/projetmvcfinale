using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Categorie
    {
        public Categorie()
        {
            Exercice = new HashSet<Exercice>();
            NoteDeCours = new HashSet<NoteDeCours>();
            SousCategorie = new HashSet<SousCategorie>();
        }
        [Display(Name = "Numéro")]
        public int IdCateg { get; set; }
        [Display(Name = "Catégorie")]
        public string NomCategorie { get; set; }

        [Display(Name = "Exercice")]
        public ICollection<Exercice> Exercice { get; set; }
        [Display(Name = "Document")]
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
        [Display(Name = "Sous-catégorie")]
        public ICollection<SousCategorie> SousCategorie { get; set; }
    }
}
