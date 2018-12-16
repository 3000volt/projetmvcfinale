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
        [Display(Name = "Titre")]
        [Required]
        public string NomNote { get; set; }
        [Required]
        public IFormFile Lien { get; set; }
        //public DateTime DateInsertion { get; set; }
        //public string AdresseCourriel { get; set; }
        [Display(Name = "Numéro de catégorie")]
        public int IdCateg { get; set; }
        [Display(Name ="Sous-catégorie")]
        public string SousCategorie { get; set; }
    }
}
