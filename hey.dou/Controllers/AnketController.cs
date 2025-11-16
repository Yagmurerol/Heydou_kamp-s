using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization; // GÜVENLİK KALDIRILDI
//using Microsoft.AspNetCore.Identity; // GÜVENLİK KALDIRILDI
//using System.Security.Claims; // GÜVENLİK KALDIRILDI
using System.Linq;

namespace heydou.Controllers
{
    // [Authorize] // GÜVENLİK KALDIRILDI
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;
        // private readonly UserManager<IdentityUser> _userManager; // GÜVENLİK KALDIRILDI

        public AnketController(HeydouContext context) // UserManager SİLİNDİ
        {
            _context = context;
        }

        // --- 1. TÜM ANKETLERİ LİSTELE ---
        public async Task<IActionResult> Index()
        {
            var anketler = await _context.Ankets.Where(a => a.IsActive).ToListAsync();
            return View(anketler);
        }

        // --- 2. ANKET OLUŞTURMA ---
        // [Authorize(Roles = "KulupBaskani")] // GÜVENLİK KALDIRILDI
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "KulupBaskani")] // GÜVENLİK KALDIRILDI
        public async Task<IActionResult> Create([Bind("Title,Description,EndDate")] Anket anket)
        {
            if (ModelState.IsValid)
            {
                anket.IsActive = true;
                _context.Add(anket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = anket.AnketID });
            }
            return View(anket);
        }

        // --- 3. ANKET DETAYLARI ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var anket = await _context.Ankets
                .Include(a => a.AnketSecenegis)
                .FirstOrDefaultAsync(m => m.AnketID == id);
            if (anket == null) return NotFound();

            // GÜVENSİZ TEST: Herkesi "test-kullanicisi" olarak varsay
            var currentUserID = "test-kullanicisi";
            ViewBag.HasVoted = await _context.Oys.AnyAsync(o => o.AnketID == id && o.UserID == currentUserID);
            return View(anket);
        }

        // --- 4. SEÇENEK EKLEME ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "KulupBaskani")] // GÜVENLİK KALDIRILDI
        public async Task<IActionResult> AddOption(int anketID, string secenekText)
        {
            if (!string.IsNullOrEmpty(secenekText))
            {
                var anketSecenegi = new AnketSecenegi { AnketID = anketID, SecenekText = secenekText };
                _context.Add(anketSecenegi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = anketID });
        }

        // --- 5. OY VERME ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Ogrenci")] // GÜVENLİK KALDIRILDI
        public async Task<IActionResult> Vote(int anketID, int secenekID)
        {
            // GÜVENSİZ TEST: Oy veren kişiyi "test-kullanicisi" olarak kaydet
            var currentUserID = "test-kullanicisi";

            bool hasVoted = await _context.Oys.AnyAsync(o => o.AnketID == anketID && o.UserID == currentUserID);

            if (hasVoted)
            {
                TempData["Error"] = "Bu ankete zaten oy verdiniz.";
                return RedirectToAction(nameof(Details), new { id = anketID });
            }

            var oy = new Oy { AnketID = anketID, SecenekID = secenekID, UserID = currentUserID };
            _context.Add(oy);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Oyunuz kaydedildi!";
            return RedirectToAction(nameof(Details), new { id = anketID });
        }

        // --- 6. ANKETİ SONUÇLANDIR (5. Fonksiyonun "Sonuçlanması") ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "KulupBaskani")] // GÜVENLİK KALDIRILDI
        public async Task<IActionResult> Finalize(int id)
        {
            var anket = await _context.Ankets
                .Include(a => a.AnketSecenegis).ThenInclude(s => s.Oys)
                .FirstOrDefaultAsync(a => a.AnketID == id);
            if (anket == null) return NotFound();

            var kazananSecenek = anket.AnketSecenegis.OrderByDescending(s => s.Oys.Count).FirstOrDefault();
            if (kazananSecenek == null)
            {
                TempData["Error"] = "Oylama sonucu bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            // KAZANAN SONUCU "Event" TABLOSUNA KAYDET (Duyurma adımı)
            var yeniEtkinlik = new Event
            {
                Title = kazananSecenek.SecenekText,
                Description = anket.Title + " anketinin sonucudur.",
                EventDate = anket.EndDate.AddDays(7), // Örnek tarih
                Location = "Belirlenecek",
                AnketID = anket.AnketID
            };
            _context.Add(yeniEtkinlik);

            // Anketi kapat
            anket.IsActive = false;
            await _context.SaveChangesAsync();

            // 5. Fonksiyonun "duyurulması" için 6. fonksiyonun ana sayfasına yönlendir.
            // Bu, 'EventsController' adında bir dosyanızın olmasını gerektirir.
            return RedirectToAction("Index", "Events");
        }
    }
}