using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // Session işlemleri için

namespace hey.dou.Controllers
{
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;

        public AnketController(HeydouContext context)
        {
            _context = context;
        }

        // Yardımcı metot: En son anketi getir
        private async Task<Anket?> GetCurrentPollAsync()
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                .OrderByDescending(a => a.AnketId)
                .FirstOrDefaultAsync();
        }

        // 1. YENİ ANKET OLUŞTURMA (Sayfa)
        public IActionResult Create()
        {
            // Giriş Kontrolü
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // 2. YENİ ANKET KAYDETME (İşlem)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Anket anket)
        {
            // Giriş Kontrolü
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(anket.Title))
            {
                ModelState.AddModelError("Title", "Başlık alanı zorunludur.");
                return View(anket);
            }

            // SÜRE AYARI: 1 Gün
            anket.EndDate = DateTime.Now.AddDays(1);
            anket.IsActive = true;

            _context.Ankets.Add(anket);
            await _context.SaveChangesAsync();

            var secenekler = new List<AnketSecenegi>();

            // Başlığı seçenek olarak ekle
            if (!string.IsNullOrWhiteSpace(anket.Title))
            {
                secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = anket.Title });
            }

            // Diğer standart seçenekler
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Hafta Sonu Doğa Yürüyüşü" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Kutu Oyunu Gecesi" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Topluluk Gönüllülük Günü" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Kodlama Atölyesi" });

            _context.AnketSecenegis.AddRange(secenekler);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = anket.AnketId });
        }

        // 3. ANKET DETAYI / OYLAMA
        public async Task<IActionResult> Details(int? id)
        {
            // --- GİRİŞ KONTROLÜ ---
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Oy.cs modelin KullaniciId'yi string istiyor, çeviriyoruz.
            string currentUserID = userIdFromSession.Value.ToString();
            // ----------------------

            var anket = (id == null)
                ? await GetCurrentPollAsync()
                : await _context.Ankets
                            .Include(a => a.AnketSecenegis)
                            .ThenInclude(s => s.Oys)
                            .FirstOrDefaultAsync(m => m.AnketId == id);

            if (anket == null)
            {
                return RedirectToAction(nameof(Create));
            }

            // Bu kullanıcı bu ankete daha önce oy vermiş mi?
            ViewBag.HasVoted = await _context.Oys
                .AnyAsync(o => o.AnketId == anket.AnketId && o.KullaniciId == currentUserID);

            return View(anket);
        }

        // 4. OY VERME İŞLEMİ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int anketId, int secenekId)
        {
            // --- GİRİŞ KONTROLÜ ---
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null) return RedirectToAction("Login", "Account");

            string currentUserID = userIdFromSession.Value.ToString();
            // ----------------------

            var anket = await _context.Ankets.FindAsync(anketId);

            if (anket == null || anket.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Bu anket süresi dolduğu için oy verilemez.";
                return RedirectToAction(nameof(Details), new { id = anketId });
            }

            bool hasVoted = await _context.Oys
                .AnyAsync(o => o.AnketId == anketId && o.KullaniciId == currentUserID);

            if (hasVoted)
            {
                TempData["Error"] = "Bu ankete zaten oy verdiniz.";
                return RedirectToAction(nameof(Details), new { id = anketId });
            }

            var oy = new Oy
            {
                AnketId = anketId,
                SecenekId = secenekId,
                KullaniciId = currentUserID // Gerçek giriş yapan kişinin ID'si
            };

            _context.Oys.Add(oy);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Oyunuz kaydedildi!";
            return RedirectToAction(nameof(Details), new { id = anketId });
        }

        // 5. LİSTELEME
        public async Task<IActionResult> Index()
        {
            // Giriş kontrolü
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            var anketler = await _context.Ankets.OrderByDescending(a => a.AnketId).ToListAsync();
            return View(anketler);
        }
    }
}