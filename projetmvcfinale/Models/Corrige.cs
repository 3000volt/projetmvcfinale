using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class Corrige
    {
        public Corrige()
        {
            Exercice = new HashSet<Exercice>();
        }

        public int Idcorrige { get; set; }
        public string CorrigeDocNom { get; set; }
        public string Lien { get; set; }
        public DateTime DateInsertion { get; set; }
        public int Idexercice { get; set; }

        public Exercice IdexerciceNavigation { get; set; }
        public ICollection<Exercice> Exercice { get; set; }
    }
}
