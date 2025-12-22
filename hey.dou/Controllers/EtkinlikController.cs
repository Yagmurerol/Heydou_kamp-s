using hey.dou.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
    [Authorize]
    public class EtkinlikController : Controller
    {
        private readonly HeydouContext _context;

        public EtkinlikController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        public async Task<IActionResult> Index() // Anket sonucu seçilen aktif etkinlikleri listeler
        {
            var secilenEtkinlikler = await _context.Events
                                               .Where(e => e.AnketSonucuSecildi.GetValueOrDefault() == true)
                                               .ToListAsync();

            return View(secilenEtkinlikler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KatilimiKaydet(int etkinlikId, bool katiliyor) // Kullanıcının bir etkinliğe katılım durumunu (evet/hayır) kaydeder veya günceller
        {
            var kullaniciIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(kullaniciIdString))
            {
                return Unauthorized();
            }

            if (!int.TryParse(kullaniciIdString, out int kullaniciId))
            {
                return BadRequest("Hatalı Kullanıcı ID formatı.");
            }

            var katilimKaydi = await _context.Katilimlars
                                             .FirstOrDefaultAsync(k => k.EtkinlikId == etkinlikId && k.KullaniciId == kullaniciId);

            if (katilimKaydi == null)
            {
                katilimKaydi = new Katilim
                {
                    EtkinlikId = etkinlikId,
                    KullaniciId = kullaniciId,
                    KatilimDurumu = katiliyor,
                    Etkinlik = null!
                };
                _context.Katilimlars.Add(katilimKaydi);
            }
            else
            {
                katilimKaydi.KatilimDurumu = katiliyor;
                _context.Katilimlars.Update(katilimKaydi);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Katilimcilar), new { id = etkinlikId });
        }

        public async Task<IActionResult> Katilimcilar(int id) // Belirli bir etkinliğe katılacağını bildiren kullanıcıların listesini görüntüler
        {
            var etkinlik = await _context.Events.FindAsync(id);

            if (etkinlik == null || etkinlik.KatilimOylamasiAcik.GetValueOrDefault() == false)
            {
                return NotFound();
            }

            var katilimcilar = await _context.Katilimlars
                                             .Where(k => k.EtkinlikId == id && k.KatilimDurumu.GetValueOrDefault() == true)
                                             .ToListAsync();

            ViewBag.EtkinlikAd = etkinlik.Title;

            return View(katilimcilar);
        }
    }
}