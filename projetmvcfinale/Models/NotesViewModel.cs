using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class NotesViewModel
    {
        //public int IdDocument { get; set; }
        public string NomNote { get; set; }
        public IFormFile Lien { get; set; }
        //public DateTime DateInsertion { get; set; }
        //public string AdresseCourriel { get; set; }
        public int IdCateg { get; set; }
        public string SousCategorie { get; set; }
    }
}
