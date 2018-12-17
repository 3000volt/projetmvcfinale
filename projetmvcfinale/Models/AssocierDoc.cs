using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class AssocierDoc
    {
        [Display(Name ="Exercice")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public int Idexercice { get; set; }
        [Display(Name = "Document")]
        [Required(ErrorMessage = "Ce champs est requis")]
        public int IdDocument { get; set; }
    }
}
