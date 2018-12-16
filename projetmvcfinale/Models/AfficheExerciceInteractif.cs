using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class AfficheExerciceInteractif
    {
        [Display(Name = "Exercice")]
        public Exercice exercie { get; set; }
        [Display(Name = "Ligne")]
        public List<LignePerso> lignesExercice { get; set; }
    }
}
