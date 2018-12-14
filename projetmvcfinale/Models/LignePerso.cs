using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class LignePerso
    {
        [Display(Name = "Numéro")]
        public int NumeroQuestion { get; set; }
        public string Ligne { get; set; }
        [Display(Name = "Choix de réponse")]
        public List<ChoixDeReponse> listeChoixReponses { get; set; }
        [Display(Name = "Choix de réponse")]
        public List<ChoixDeReponseTest> listeChoixReponses2 { get; set; }
    }
}
