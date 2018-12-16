using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class InsertionExercice
    {
        [Display(Name = "Exercice")]
        public Exercice exercice { get; set; }

        //public SortedList<int, LignePerso> listeLignes { get; set; }
        [Display(Name = "Lignes")]
        public List<LignePerso> listeLignes { get; set; }
    }
}
