using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class AfficheExerciceInteractif
    {
        public Exercice exercie { get; set; }
        public List<LignePerso> lignesExercice { get; set; }
    }
}
