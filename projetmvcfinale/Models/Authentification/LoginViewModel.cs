using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models.Authentification
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Courriel")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Se souvenir de moi?")]
        public bool RememberMe { get; set; }

    }
}
