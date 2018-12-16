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
       
        public int IdSousCategorie { get; set; }
        [Display(Name = "sous-catégorie")]
        public string NomSousCategorie { get; set; }
        public int IdCateg { get; set; }

        public Categorie IdCategNavigation { get; set; }
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
    }
}
