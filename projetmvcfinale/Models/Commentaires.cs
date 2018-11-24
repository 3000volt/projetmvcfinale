using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Commentaires
    {
        [Display(Name = "Identifiant")]
        public int IdCom { get; set; }
        [Display(Name = "Commentaire")]
        public string TexteCom { get; set; }
        [Display(Name = "Date d'insertion")]
        public DateTime DateCommentaire { get; set; }
        [Display(Name = "Courriel")]
        public string AdresseCourriel { get; set; }

        public Utilisateur AdresseCourrielNavigation { get; set; }
    }
}
