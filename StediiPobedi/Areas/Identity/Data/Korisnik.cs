using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StediiPobedi.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Korisnik class
    public class Korisnik : IdentityUser
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
    }
}
