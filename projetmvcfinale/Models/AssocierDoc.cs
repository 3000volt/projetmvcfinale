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
        public int Idexercice { get; set; }
        [Display(Name = "Document")]
        public int IdDocument { get; set; }
    }
}
