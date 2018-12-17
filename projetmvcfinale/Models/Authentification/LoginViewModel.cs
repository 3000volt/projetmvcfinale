using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models.Authentification
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Ce champs est obligatoire")]
        [Display(Name = "Courriel")]
        [EmailAddress(ErrorMessage = "Ce email est invalide")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password,ErrorMessage ="Ce mot de passe est incorect")]
        public string Password { get; set; }

        [Display(Name = "Se souvenir de moi?")]
        public bool RememberMe { get; set; }

    }
}
