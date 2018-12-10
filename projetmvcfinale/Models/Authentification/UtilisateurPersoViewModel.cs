using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models.Authentification
{
    public class UtilisateurPersoViewModel
    {

        [Display(Name = "Adresse Courriel")]
        public string AdresseCourriel { get; set; }
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Display(Name = "Date d'enregistrement")]
        public DateTime RegistrerDate { get; set; }

        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
        
    }
}
