using System;
using System.Collections.Generic;

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

        public string AdresseCourriel { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime RegistrerDate { get; set; }

        public ICollection<Commentaires> Commentaires { get; set; }
        public ICollection<Exercice> Exercice { get; set; }
        public ICollection<NoteDeCours> NoteDeCours { get; set; }
    }
}
