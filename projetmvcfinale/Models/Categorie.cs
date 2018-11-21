using System;
using System.Collections.Generic;

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

        public int IdCateg { get; set; }
        public string NomCategorie { get; set; }

        public ICollection<Exercice> Exercice { get; set; }
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
        public ICollection<SousCategorie> SousCategorie { get; set; }
    }
}
