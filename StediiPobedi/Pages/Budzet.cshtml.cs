using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using StediiPobedi.Areas.Identity.Data;
using StediiPobedi.Models;
using ServiceStack.Redis;
using System.Text.Json;

namespace StediiPobedi.Pages
{
    public class BudzetModel : PageModel
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private RedisClient _redis;

        [BindProperty]
        public IList<Budzet> Budzeti { get; set; }
        [BindProperty]
        public Budzet Budzet { get; set; }
        public Dictionary<string, string> dicRashodi { get; set; }
        public Dictionary<string, string> dicBudzeti { get; set; }
        [BindProperty]
        public IList<Rashod> Rashodi { get; set; }
        [BindProperty]
        public IList<Rashod> DodatiRashodi { get; set; }


        public BudzetModel(UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _redis = new RedisClient(Config.SingleHost);
            Budzeti = new List<Budzet>();
            dicRashodi = new Dictionary<string, string>();
            Rashodi = new List<Rashod>();
            DodatiRashodi = new List<Rashod>();
            Budzet = new Budzet();
        }
        public async Task OnGetAsync()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            dicRashodi = _redis.GetAllEntriesFromHash("{" + korisnik.UserName + "}-rashodi");
            foreach (KeyValuePair<string, string> key in dicRashodi)
            {
                Rashod r = JsonSerializer.Deserialize<Rashod>(key.Value);
                Rashodi.Add(r);
            }

            dicBudzeti = _redis.GetAllEntriesFromHash("{" + korisnik.UserName + "}-budzeti");
            foreach (KeyValuePair<string, string> key in dicBudzeti)
            {
                Budzet b = JsonSerializer.Deserialize<Budzet>(key.Value);
                Budzeti.Add(b);
            }
        }
        public async Task<IActionResult> OnPostDodajBudzet()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            int id = -1;
            List<string> keys = _redis.GetHashKeys("{" + korisnik.UserName + "}-budzeti");
            if (keys.Count != 0)
            {
                id = Int32.Parse(keys[keys.Count - 1]);
            }

            Dictionary<string, string> hmset = new Dictionary<string, string>();

            id++;

            Budzet.Id = id;

            int ukupno = 0;
            foreach (Rashod r in Rashodi)
            {
                if (r.Selektovan == true)
                {
                    Budzet.Rashodi.Add(r);
                    ukupno += Int32.Parse(r.Iznos);
                }
                else
                {
                    Budzet.DodatiRashodi.Add(r);
                }
            }
            Budzet.UkupniRashodi = ukupno.ToString();

            int preostalo = Int32.Parse(Budzet.Iznos) - ukupno;

            Budzet.PreostaliIznos = preostalo.ToString();

            int procenat = (100 * preostalo) / Int32.Parse(Budzet.Iznos);

            Budzet.Procenat = procenat.ToString();

            string json = JsonSerializer.Serialize(Budzet);
            hmset.Add(Budzet.Id.ToString(), json);

            _redis.SetRangeInHash("{" + korisnik.UserName + "}-budzeti", hmset);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostIzmeniBudzet(int id, int br)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            Budzet novi = new Budzet();
            novi.Id = id;
            novi.Opis = Budzeti[br].Opis;
            novi.Iznos = Budzeti[br].Iznos;

            int ukupno = 0;
            foreach (Rashod r in Budzeti[br].Rashodi)
            {
                if (r.Selektovan == true)
                {
                    novi.Rashodi.Add(r);
                    ukupno += Int32.Parse(r.Iznos);
                }
                else
                {
                    novi.DodatiRashodi.Add(r);
                }
            }
            foreach (Rashod r in Budzeti[br].DodatiRashodi)
            {
                if (r.Selektovan == true)
                {
                    novi.Rashodi.Add(r);
                    ukupno += Int32.Parse(r.Iznos);
                }
                else
                {
                    novi.DodatiRashodi.Add(r);
                }
            }
            novi.UkupniRashodi = ukupno.ToString();

            int preostalo = Int32.Parse(novi.Iznos) - ukupno;

            novi.PreostaliIznos = preostalo.ToString();

            int procenat = (100 * preostalo) / Int32.Parse(novi.Iznos);

            novi.Procenat = procenat.ToString();

            Dictionary<string, string> hmset = new Dictionary<string, string>();

            string json = JsonSerializer.Serialize(novi);
            hmset.Add(id.ToString(), json);

            _redis.SetRangeInHash("{" + korisnik.UserName + "}-budzeti", hmset);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUkloniBudzet(int id)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            _redis.RemoveEntryFromHash("{" + korisnik.UserName + "}-budzeti", id.ToString());

            return RedirectToPage();
        }
    }
}
