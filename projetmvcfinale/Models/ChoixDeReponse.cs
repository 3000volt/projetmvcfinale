using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class ChoixDeReponse
    {
        public int IdChoix { get; set; }
        public string ChoixDeReponse1 { get; set; }
        public bool Response { get; set; }
        public int NoOrdre { get; set; }
        public int IdLigne { get; set; }

        public LigneTestInteractif IdLigneNavigation { get; set; }
    }
}
