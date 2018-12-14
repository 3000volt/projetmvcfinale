using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projetmvcfinale.Models
{
    public partial class Corrige
    {
        public Corrige()
        {
            Exercice = new HashSet<Exercice>();
        }
        [Display(Name = "Numéro de corrigé")]
        public int Idcorrige { get; set; }
        [Display(Name = "Titre")]
        public string CorrigeDocNom { get; set; }
        public string Lien { get; set; }
        [Display(Name = "Date d'insertion")]
        public DateTime DateInsertion { get; set; }
        [Display(Name = "Exercice associé")]
        public int Idexercice { get; set; }

        [Display(Name = "Exercice associé")]
        public Exercice IdexerciceNavigation { get; set; }
        public ICollection<Exercice> Exercice { get; set; }
    }
}
