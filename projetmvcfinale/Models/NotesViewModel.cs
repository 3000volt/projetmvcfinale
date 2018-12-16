using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class NotesViewModel
    {
        //public int IdDocument { get; set; }
        [Display(Name = "Nom : ")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public string NomNote { get; set; }
        [Display(Name = "Lien : ")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public IFormFile Lien { get; set; }
        [Display(Name = "Catégorie : ")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public int IdCateg { get; set; }
        [Display(Name = "Sous-Catégorie : ")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public string SousCategorie { get; set; }
    }
}
