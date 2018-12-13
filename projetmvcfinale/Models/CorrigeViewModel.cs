using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projetmvcfinale.Models
{
    public class CorrigeViewModel
    {
        
        public string CorrigeDocNom { get; set; }
        public IFormFile Lien { get; set; }
        public int Idexercice { get; set; }
    }
}
