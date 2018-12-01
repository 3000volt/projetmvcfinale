using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class NoteDeCours
    {
        public NoteDeCours()
        {
            Exercice = new HashSet<Exercice>();
        }

        public int IdDocument { get; set; }
        public string NomNote { get; set; }
        public string Lien { get; set; }
        public DateTime DateInsertion { get; set; }
        public string AdresseCourriel { get; set; }
        public int IdCateg { get; set; }
        public int IdSousCategorie { get; set; }

        public Utilisateur AdresseCourrielNavigation { get; set; }
        public Categorie IdCategNavigation { get; set; }
        public SousCategorie IdSousCategorieNavigation { get; set; }
        public ICollection<Exercice> Exercice { get; set; }
    }
}
