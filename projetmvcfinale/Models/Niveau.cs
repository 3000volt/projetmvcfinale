using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class Niveau
    {
        public Niveau()
        {
            Exercice = new HashSet<Exercice>();
        }

        public int IdDifficulte { get; set; }
        public string NiveauDifficulte { get; set; }

        public ICollection<Exercice> Exercice { get; set; }
    }
}
