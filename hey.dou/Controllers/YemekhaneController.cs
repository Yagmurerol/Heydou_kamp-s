using hey.dou.Models;
using HeyDOU.KampusApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeyDOU.KampusApp.Controllers // Namespace'inizi kontrol edin
{
    public class YemekhaneController : Controller
    {
        private readonly HeydouContext _context;

        // DbContext'i buraya enjekte ediyoruz (Program.cs'te tanımladınız).
        public YemekhaneController(HeydouContext context)
        {
            _context = context;
        }

        // GET: /Yemekhane/Index (Menüleri Listeleme)
        public async Task<IActionResult> Index()
        {
            // Veritabanındaki TÜM menüleri çek (Filtreleme ve sıralama şimdilik kaldırıldı)
            var gelecekMenuler = await _context.HaftalikMenus.ToListAsync();
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
        public async Task<IActionResult> MenuEkle(HaftalikMenu menu)
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