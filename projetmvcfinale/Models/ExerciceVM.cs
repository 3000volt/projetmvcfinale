using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class ExerciceVM
    {
        public string NomExercices { get; set; }
        public string TypeExercice { get; set; }
        public int IdDifficulte { get; set; }
        public int IdCateg { get; set; }
    }
}
