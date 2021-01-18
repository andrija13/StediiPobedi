using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StediiPobedi.Models
{
    public class Rashod
    {
        public int Id { get; set; }
        public string Tip { get; set; }
        public string Opis { get; set; }
        public string Iznos { get; set; }
        public bool Selektovan { get; set; }
    }
}
