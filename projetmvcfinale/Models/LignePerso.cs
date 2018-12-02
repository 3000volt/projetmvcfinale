using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class LignePerso
    {
        public int NumeroQuestion { get; set; }
        public string Ligne { get; set; }
        public List<ChoixDeReponse> listeChoixReponses { get; set; }
        public List<ChoixDeReponseTest> listeChoixReponses2 { get; set; }
    }
}
