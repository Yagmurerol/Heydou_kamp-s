using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace hey.dou.Controllers
{
    public class KrokiController : Controller
    {
        private readonly HeydouContext _context;

        public KrokiController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        public IActionResult Index() // Kroki arama sayfasını görüntüler
        {
            return View();
        }

        [HttpGet]
        public IActionResult MekanAra(string kod) // Girilen mekan koduna göre veritabanında arama yapar ve kat/kroki bilgisini döner
        {
            if (string.IsNullOrEmpty(kod))
            {
                return BadRequest(new { mesaj = "Lütfen bir mekan kodu girin." });
            }

            var mekan = _context.Mekanlars
                                .Include(m => m.Kat)
                                .FirstOrDefault(x => x.MekanKodu == kod);

            if (mekan == null)
            {
                return NotFound(new { mesaj = "Bu koda sahip bir yer bulunamadı. Lütfen 'B2-01' gibi tam kod giriniz." });
            }

            var sonuc = new MekanSonucModel
            {
                MekanKodu = mekan.MekanKodu,
                Aciklama = mekan.Aciklama,
                KatAdi = mekan.Kat.KatAdi,
                KrokiResimAdi = mekan.Kat.KrokiResimAdi
            };

            return Ok(sonuc);
        }
    }
}