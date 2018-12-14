using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class ChoixDeReponseTest
    {
        [Display(Name = "Choix de réponse")]
        public string ChoixDeReponse1 { get; set; }
        [Display(Name = "Réponse")]
        public bool Response { get; set; }
        [Display(Name = "Ordre")]
        public int NoOrdre { get; set; }
    }
}
