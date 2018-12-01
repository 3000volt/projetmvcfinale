using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class InsertionExercice
    {
        public Exercice exercice { get; set; }

        //public SortedList<int, LignePerso> listeLignes { get; set; }
        public List<LignePerso> listeLignes { get; set; }
    }
}
