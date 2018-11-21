using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class LigneTestInteractif
    {
        public LigneTestInteractif()
        {
            ChoixDeReponse = new HashSet<ChoixDeReponse>();
        }

        public int IdLigne { get; set; }
        public int NumeroQuestion { get; set; }
        public string Ligne { get; set; }
        public int Idexercice { get; set; }

        public Exercice IdexerciceNavigation { get; set; }
        public ICollection<ChoixDeReponse> ChoixDeReponse { get; set; }
    }
}
