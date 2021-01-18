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
using System.Text.Json.Serialization;
using EasyCaching;
using EasyCaching.Core;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StediiPobedi.Pages
{
    public class PrihodiRashodiModel : PageModel
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private RedisClient _redis;

        public Dictionary<string, string> dicPrihodi { get; set; }
        public Dictionary<string, string> dicRashodi { get; set; }

        [BindProperty]
        public IList<Prihod> Prihodi { get; set; }
        [BindProperty]
        public IList<Rashod> Rashodi { get; set; }
        [BindProperty]
        public IList<string> NoviPrihodi { get; set; }
        [BindProperty]
        public IList<string> NoviIznosi { get; set; }
        [BindProperty]
        public IList<string> NoviTipovi { get; set; }
        [BindProperty]
        public IList<string> NoviRashodi { get; set; }
        [BindProperty]
        public IList<string> IzabraniRashodi { get; set; }
        [BindProperty]
        public IList<string> NoviRashodiIznos { get; set; }

        public PrihodiRashodiModel(UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _redis = new RedisClient(Config.SingleHost);
            Prihodi = new List<Prihod>();
            Rashodi = new List<Rashod>();
            NoviPrihodi = new List<string>();
            NoviIznosi = new List<string>();
            NoviRashodi = new List<string>();
            NoviRashodiIznos = new List<string>();
            NoviTipovi = new List<string>();
            dicPrihodi = new Dictionary<string, string>();
            dicRashodi = new Dictionary<string, string>();
            IzabraniRashodi = new List<string>();
        }
        public async Task OnGetAsync()
        {
            var korisnik = await _userManager.GetUserAsync(User);

            dicPrihodi = _redis.GetAllEntriesFromHash("{" + korisnik.UserName + "}-prihodi");
            foreach(KeyValuePair<string,string> key in dicPrihodi)
            {
                Prihod p = JsonSerializer.Deserialize<Prihod>(key.Value);
                Prihodi.Add(p);
            }

            dicRashodi = _redis.GetAllEntriesFromHash("{" + korisnik.UserName + "}-rashodi");
            foreach(KeyValuePair<string,string> key in dicRashodi)
            {
                Rashod r = JsonSerializer.Deserialize<Rashod>(key.Value);
                Rashodi.Add(r);
            }
        }
        public async Task<IActionResult> OnPostSacuvajAsync()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            int id = -1;
            List<string> keys = _redis.GetHashKeys("{" + korisnik.UserName + "}-prihodi");
            if (keys.Count != 0)
            {
                id = Int32.Parse(keys[keys.Count - 1]);
            }

            Dictionary<string, string> hmset = new Dictionary<string, string>();
            for (int i =0;i<NoviPrihodi.Count;i++)
            {
                id++;
                Prihod p = new Prihod();
                p.Id = id;
                p.Naziv = NoviPrihodi[i];
                p.Iznos = NoviIznosi[i];
                string json = JsonSerializer.Serialize(p);
                hmset.Add(p.Id.ToString(), json);
            }

            _redis.SetRangeInHash("{"+korisnik.UserName+"}-prihodi", hmset);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUkloniPrihodAsync(int id)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            _redis.RemoveEntryFromHash("{" + korisnik.UserName + "}-prihodi", id.ToString());

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSacuvajRashodeAsync()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            int id = -1;
            List<string> keys = _redis.GetHashKeys("{" + korisnik.UserName + "}-rashodi");
            if (keys.Count != 0)
            {
                id = Int32.Parse(keys[keys.Count - 1]);
            }

            Dictionary<string, string> hmset = new Dictionary<string, string>();
            for (int i = 0; i < IzabraniRashodi.Count; i++)
            {
                id++;
                Rashod r = new Rashod();
                r.Id = id;
                r.Tip = IzabraniRashodi[i];
                r.Opis = NoviRashodi[i];
                r.Iznos = NoviRashodiIznos[i];
                string json = JsonSerializer.Serialize(r);
                hmset.Add(r.Id.ToString(), json);
            }

            _redis.SetRangeInHash("{" + korisnik.UserName + "}-rashodi", hmset);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUkloniRashodAsync(int id)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            _redis.RemoveEntryFromHash("{" + korisnik.UserName + "}-rashodi", id.ToString());

            return RedirectToPage();
        }

    }
}
