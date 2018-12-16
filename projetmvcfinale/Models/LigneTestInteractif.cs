using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class LigneTestInteractif
    {
        public LigneTestInteractif()
        {
            ChoixDeReponse = new HashSet<ChoixDeReponse>();
        }

        [Display(Name = "Ligne")]
        public int IdLigne { get; set; }
        [Display(Name = "Numéro de la question")]
        public int NumeroQuestion { get; set; }
        public string Ligne { get; set; }
        [Display(Name = "Exercice associé")]
        public int Idexercice { get; set; }

        public Exercice IdexerciceNavigation { get; set; }
        public ICollection<ChoixDeReponse> ChoixDeReponse { get; set; }
    }
}
