using Microsoft.AspNetCore.Mvc;
using HeyDOU.KampusApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HeyDOU.KampusApp.Controllers // Namespace'inizi kontrol edin
{
    public class YemekhaneController : Controller
    {
        private readonly ApplicationDbContext _context;

        // DbContext'i buraya enjekte ediyoruz (Program.cs'te tanımladınız).
        public YemekhaneController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Yemekhane/Index (Menüleri Listeleme)
        public async Task<IActionResult> Index()
        {
            var bugun = DateTime.Today;
            var gelecekMenuler = await _context.YemekhaneMenuleri
                                               .Where(m => m.Tarih >= bugun)
                                               .OrderBy(m => m.Tarih)
                                               .ToListAsync();
            return View(gelecekMenuler);
        }

        // GET: /Yemekhane/MenuEkle (Menü Ekleme Formunu Gösterme)
        public IActionResult MenuEkle()
        {
            return View();
        }

        // POST: Form Gönderimini İşleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEkle(YemekhaneMenu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Kayıt sonrası listeye yönlendir
            }
            return View(menu); // Hata varsa formu tekrar göster
        }
    }
}