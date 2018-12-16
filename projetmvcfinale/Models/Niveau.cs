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
        [Display(Name = "Numéro de difficulté")]
        public int IdDifficulte { get; set; }
        [Display (Name ="Niveau de difficulté")]
        [Required]
      
        public string NiveauDifficulte { get; set; }

        public ICollection<Exercice> Exercice { get; set; }
    }
}
