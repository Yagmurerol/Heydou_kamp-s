using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace hey.dou.Controllers
{
    public class KrokiController : Controller
    {
        private readonly HeydouContext _context;

        public KrokiController(HeydouContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MekanAra(string kod)
        {
            if (string.IsNullOrEmpty(kod))
            {
                return BadRequest(new { mesaj = "Lütfen bir mekan kodu girin." });
            }

            // Veritabanı tablosu "Mekanlars" olduğu için s takısı önemli
            var mekan = _context.Mekanlars
                                .Include(m => m.Kat)
                                .FirstOrDefault(x => x.MekanKodu == kod);

            if (mekan == null)
            {
                return NotFound(new { mesaj = "Bu koda sahip bir yer bulunamadı. Lütfen 'B2-01' gibi tam kod giriniz." });
            }

            // Veritabanındaki resim adını (kat_D1.png) olduğu gibi gönderiyoruz
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