using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // List kullanmak için eklendi

namespace heydou.Controllers
{
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;

        public AnketController(HeydouContext context)
        {
            _context = context;
        }

        // === 1. KULÜP BAŞKANI: YENİ ETKİNLİK OLUŞTUR SAYFASI ===
        // GET: /Anket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Anket/Create
        // GÜNCELLEME: Anket başlığı artık bir seçenek olarak kaydediliyor.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,EndDate")] Anket anket)
        {
            if (!ModelState.IsValid)
            {
                return View(anket);
            }

            anket.IsActive = true;
            _context.Ankets.Add(anket);
            await _context.SaveChangesAsync();

            // Anket seçenekleri listesi oluşturuluyor
            var secenekler = new List<AnketSecenegi>();

            // YENİ EKLEME: Kullanıcının girdiği Etkinlik Adını (Anket.Title) bir seçenek olarak ekle
            if (!string.IsNullOrWhiteSpace(anket.Title))
            {
                // Kullanıcının girdiği etkinlik adı artık oy verilebilir bir seçenek
                secenekler.Add(new AnketSecenegi { AnketID = anket.AnketID, SecenekText = anket.Title });
            }

            // 4 tane sabit etkinlik seçeneğini ekle
            secenekler.Add(new AnketSecenegi { AnketID = anket.AnketID, SecenekText = "Hafta Sonu Doğa Yürüyüşü" });
            secenekler.Add(new AnketSecenegi { AnketID = anket.AnketID, SecenekText = "Kutu Oyunu Gecesi" });
            secenekler.Add(new AnketSecenegi { AnketID = anket.AnketID, SecenekText = "Topluluk Gönüllülük Günü" });
            secenekler.Add(new AnketSecenegi { AnketID = anket.AnketID, SecenekText = "Kodlama Atölyesi" });


            _context.AnketSecenegis.AddRange(secenekler);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Poll));
        }

        // Yardımcı: En son oluşturulan anketi getir
        private async Task<Anket?> GetCurrentPollAsync()
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                .OrderByDescending(a => a.AnketID)
                .FirstOrDefaultAsync();
        }

        // === 2. KATILIMCI: OY VER + GRAFİK EKRANI ===
        // GET: /Anket/Poll
        public async Task<IActionResult> Poll()
        {
            var anket = await GetCurrentPollAsync();
            if (anket == null)
            {
                // Hiç anket yoksa kulüp başkanı sayfasına yönlendir
                return RedirectToAction(nameof(Create));
            }

            var currentUserID = "test-kullanicisi"; // şimdilik sabit

            ViewBag.HasVoted = await _context.Oys
                .AnyAsync(o => o.AnketID == anket.AnketID && o.UserID == currentUserID);

            return View(anket); // Views/Anket/Poll.cshtml
        }

        // === 3. KATILIMCI: OY VERME ===
        // POST: /Anket/Vote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int anketID, int secenekID)
        {
            var anket = await _context.Ankets.FindAsync(anketID);

            // Oy vermeden önce bitiş tarihi kontrolü yapılıyor.
            if (anket == null || anket.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Bu anket süresi dolduğu için oy verilemez.";
                return RedirectToAction(nameof(Poll));
            }

            var currentUserID = "test-kullanicisi";

            bool hasVoted = await _context.Oys
                .AnyAsync(o => o.AnketID == anketID && o.UserID == currentUserID);

            if (hasVoted)
            {
                TempData["Error"] = "Bu ankete zaten oy verdiniz.";
                return RedirectToAction(nameof(Poll));
            }

            var oy = new Oy
            {
                AnketID = anketID,
                SecenekID = secenekID,
                UserID = currentUserID
            };

            _context.Oys.Add(oy);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Oyunuz kaydedildi!";
            return RedirectToAction(nameof(Poll));
        }
    }
}