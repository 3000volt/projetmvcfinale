using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class ExerciceVM
    {
        public int IdExercice { get; set; }
        [Display(Name ="Nom de l'exercice")]
        [Required]
        public string NomExercices { get; set; }
        [Display(Name = "Type d'exercice")]
        public string TypeExercice { get; set; }
        [Display(Name = "Niveau de difficulté")]
        public int IdDifficulte { get; set; }
        [Display(Name = "Catégorie")]
        public int IdCateg { get; set; }
        public IFormFile Lien { get; set; }
    }
}
