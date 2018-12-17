using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class CorrigeViewModel
    {
        public int idcorrige { get; set; }
        [Display(Name ="Titre")]
        [Required (ErrorMessage="Champs obligatoire")]
        public string CorrigeDocNom { get; set; }
        public IFormFile Lien { get; set; }
        [Display(Name = "Exercice associé")]
        [Required(ErrorMessage = "Champs obligatoire")]
        public int Idexercice { get; set; }
    }
}
