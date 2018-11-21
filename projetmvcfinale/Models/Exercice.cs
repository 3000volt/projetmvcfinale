using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class Exercice
    {
        public Exercice()
        {
            Corrige = new HashSet<Corrige>();
            LigneTestInteractif = new HashSet<LigneTestInteractif>();
        }

        public int Idexercice { get; set; }
        public string NomExercices { get; set; }
        public string Lien { get; set; }
        public DateTime DateInsertion { get; set; }
        public string TypeExercice { get; set; }
        public string AdresseCourriel { get; set; }
        public int IdDifficulte { get; set; }
        public int? Idcorrige { get; set; }
        public int IdDocument { get; set; }
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
