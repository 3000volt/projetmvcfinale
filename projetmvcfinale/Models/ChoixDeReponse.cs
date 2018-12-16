using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class ChoixDeReponse
    {
        [Display(Name="Choix")]
        public int IdChoix { get; set; }
        [Display(Name = "Choix de réponse")]
        public string ChoixDeReponse1 { get; set; }
        [Display(Name = "Réponse")]
        public bool Response { get; set; }
        [Display(Name = "Ordre")]
        public int NoOrdre { get; set; }
        [Display(Name = "Ligne")]
        public int IdLigne { get; set; }
      
        public LigneTestInteractif IdLigneNavigation { get; set; }
    }
}
