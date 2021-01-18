using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StediiPobedi.Models
{
    public class Budzet
    {
        public int Id { get; set; }
        public string Opis { get; set; }
        public string Iznos { get; set; }
        public string UkupniRashodi { get; set; }
        public string PreostaliIznos { get; set; }
        public string Procenat { get; set; }
        public List<Rashod> Rashodi { get; set; }
        public List<Rashod> DodatiRashodi { get; set; }

        public Budzet()
        {
            Rashodi = new List<Rashod>();
            DodatiRashodi = new List<Rashod>();
        }
    }
}
