using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class testInteractifViewModel
    {
        [Display(Name = "Numéro de question")]
        //[Required]
        public string NumeroQuestion { get; set; }
        //[Required]
        public string Ligne { get; set; }
        [Display(Name = "Choix de réponse")]
        //[Required]
        public string ChoixDeReponse { get; set; }
        //public List<string> ChoixDeReponse { get; set; }
    }
}
