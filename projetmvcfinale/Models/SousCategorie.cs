using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class SousCategorie
    {
        public SousCategorie()
        {
            NoteDeCours = new HashSet<NoteDeCours>();
        }
        [Display(Name = "Identifiant")]
        public int IdSousCategorie { get; set; }
        [Display(Name = "Sous-catégorie")]
        public string NomSousCategorie { get; set; }
        [Display(Name = "Catégorie")]
        public int IdCateg { get; set; }

        [Display(Name = "Catégorie")]
        public Categorie IdCategNavigation { get; set; }
        [Display(Name = "Note de cours")]
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
    }
}
