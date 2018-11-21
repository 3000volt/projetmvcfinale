using System;
using System.Collections.Generic;

namespace projetmvcfinale.Models
{
    public partial class Commentaires
    {
        public int IdCom { get; set; }
        public string TexteCom { get; set; }
        public DateTime DateCommentaire { get; set; }
        public string AdresseCourriel { get; set; }

        public Utilisateur AdresseCourrielNavigation { get; set; }
    }
}
