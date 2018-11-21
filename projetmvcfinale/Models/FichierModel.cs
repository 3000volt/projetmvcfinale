using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class FichierModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Fichier")]
        [Required(ErrorMessage = "Veuillez sélectionner un fichier a téléverser")]
        public string Fichier { get; set; }
    }
}
