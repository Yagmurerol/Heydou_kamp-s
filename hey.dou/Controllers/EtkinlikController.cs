using hey.dou.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class EtkinlikController : Controller
{
    private readonly HeydouContext _context;

    public EtkinlikController(HeydouContext context)
    {
        _context = context;
    }

    // --- Etkinlik Listesi ---
    public async Task<IActionResult> Index()
    {
        var secilenEtkinlikler = await _context.Events
                                               .Where(e => e.AnketSonucuSecildi == true)
                                               .ToListAsync();

        return View(secilenEtkinlikler);
    }


    // --- Katılımı Kaydetme ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KatilimiKaydet(int etkinlikId, bool katiliyor)
    {
        var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(kullaniciId))
        {
            return Unauthorized();
        }

        var katilimKaydi = await _context.Katilimlar
                                          .FirstOrDefaultAsync(k => k.EtkinlikId == etkinlikId && k.KullaniciId == kullaniciId);

        if (katilimKaydi == null)
        {
            // DÜZELTME: Modeldeki KullaniciId ve Etkinlik = null! ataması korundu.
            katilimKaydi = new Katilim
            {
                EtkinlikId = etkinlikId,
                KullaniciId = kullaniciId,
                KatilimDurumu = katiliyor,
                Etkinlik = null!
            };
            _context.Katilimlar.Add(katilimKaydi);
        }
        else
        {
            katilimKaydi.KatilimDurumu = katiliyor;
            _context.Katilimlar.Update(katilimKaydi);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Katilimcilar), new { id = etkinlikId });
    }

    // --- Katılımcıları Görüntüleme ---
    public async Task<IActionResult> Katilimcilar(int id)
    {
        // DÜZELTME: Find metodu EventId'ye göre arama yapacaktır.
        var etkinlik = await _context.Events.FindAsync(id);

        if (etkinlik == null || etkinlik.KatilimOylamasiAcik == false)
        {
            return NotFound();
        }

        // DÜZELTME: Sorgular doğru KullaniciId alanını kullanır.
        var katilimcilar = await _context.Katilimlar
                                         .Where(k => k.EtkinlikId == id && k.KatilimDurumu == true)
                                         .ToListAsync();

        ViewBag.EtkinlikAd = etkinlik.Title;

        return View(katilimcilar);
    }
}