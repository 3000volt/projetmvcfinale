using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Niveau
    {
        public Niveau()
        {
            Exercice = new HashSet<Exercice>();
        }
        public int IdDifficulte { get; set; }
        [Display(Name = "Niveau")]
        public string NiveauDifficulte { get; set; }

        public ICollection<Exercice> Exercice { get; set; }
    }
}
