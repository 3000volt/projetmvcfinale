using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models.Authentification
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [StringLength(50, ErrorMessage = "Le {0} doit être au moins {2} et maximum {1} caractères de long.", MinimumLength = 4)]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [StringLength(50, ErrorMessage = "Le {0} doit être au moins {2} et maximum {1} caractères de long.", MinimumLength = 4)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [StringLength(50, ErrorMessage = "Le {0} doit être au moins {2} et maximum {1} caractères de long.", MinimumLength = 4)]
        [DataType(DataType.Password, ErrorMessage = "Ce mot de passe est incorect")]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [DataType(DataType.Password,ErrorMessage = "Ce mot de passe est incorect")]
        [Display(Name = "Confirmez votre mot de passe")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ce champs est obligatoire")]
        [EmailAddress(ErrorMessage ="Ce email est invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public List<SelectListItem> Roles { get; set; }
        public string Role { get; set; }

    }
}
