using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace heydou.Controllers
{
    // Yöneticiler için yetkilendirme gerekebilir.
    // [Authorize(Roles = "Admin")]
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;

        public AnketController(HeydouContext context)
        {
            _context = context;
        }

        // Yardımcı metot: En son oluşturulan anketi getir
        private async Task<Anket?> GetCurrentPollAsync()
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                // DÜZELTME: AnketID -> AnketId
                .OrderByDescending(a => a.AnketId)
                .FirstOrDefaultAsync();
        }

        // === 6. FONKSİYON ENTEGRASYONU: ANKET SONUCUNU BELİRLEME ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnketSonucunuBelirle(int anketId)
        {
            var anket = await _context.Ankets
                                      .Include(a => a.AnketSecenegis)
                                      .ThenInclude(s => s.Oys)
                                      .FirstOrDefaultAsync(a => a.AnketId == anketId); // DÜZELTME: AnketID -> AnketId

            if (anket == null)
            {
                TempData["Error"] = "Belirtilen anket bulunamadı.";
                return RedirectToAction(nameof(Poll));
            }

            // Hata yok: AnketSecenegis listesi doğru
            var enCokOyuAlanSecenek = anket.AnketSecenegis
                                           .OrderByDescending(s => s.Oys.Count)
                                           .FirstOrDefault();

            if (enCokOyuAlanSecenek == null)
            {
                TempData["Error"] = "Ankette hiç oy kullanılmamış veya seçenek yok.";
                return RedirectToAction(nameof(Poll));
            }

            var yeniEtkinlik = new Event
            {
                Title = enCokOyuAlanSecenek.SecenekText,
                Description = $"Bu etkinlik, '{anket.Title}' anket sonucu seçilmiştir.",
                EventDate = anket.EndDate.AddDays(7),
                Location = "Belirlenmedi",
                AnketId = anket.AnketId, // DÜZELTME: AnketID -> AnketId
                AnketSonucuSecildi = true,
                KatilimOylamasiAcik = true,
                Katilimlar = new List<Katilim>()
            };

            _context.Events.Add(yeniEtkinlik);

            var eskiSecilenEtkinlikler = await _context.Events
                                                       .Where(e => e.AnketSonucuSecildi == true && e.AnketId == anketId) // DÜZELTME: AnketID -> AnketId
                                                       .ToListAsync();

            foreach (var eskiEtkinlik in eskiSecilenEtkinlikler)
            {
                eskiEtkinlik.AnketSonucuSecildi = false;
                eskiEtkinlik.KatilimOylamasiAcik = false;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Etkinlik ({yeniEtkinlik.Title}) anket sonucu seçildi ve katılım oylamasına açıldı.";

            return RedirectToAction("Index", "Etkinlik");
        }

        // === 1. KULÜP BAŞKANI: YENİ ETKİNLİK OLUŞTUR SAYFASI ===
        public IActionResult Create()
        {
            return View();
        }

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

            var secenekler = new List<AnketSecenegi>();

            if (!string.IsNullOrWhiteSpace(anket.Title))
            {
                // DÜZELTME: AnketID -> AnketId
                secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = anket.Title });
            }

            // DÜZELTME: AnketID -> AnketId
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Hafta Sonu Doğa Yürüyüşü" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Kutu Oyunu Gecesi" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Topluluk Gönüllülük Günü" });
            secenekler.Add(new AnketSecenegi { AnketId = anket.AnketId, SecenekText = "Kodlama Atölyesi" });


            _context.AnketSecenegis.AddRange(secenekler);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Poll));
        }

        // === 2. KATILIMCI: OY VER + GRAFİK EKRANI ===
        public async Task<IActionResult> Poll()
        {
            var anket = await GetCurrentPollAsync();
            if (anket == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var currentUserID = "test-kullanicisi";

            ViewBag.HasVoted = await _context.Oys
                // DÜZELTME: AnketID -> AnketId, UserID -> KullaniciId
                .AnyAsync(o => o.AnketId == anket.AnketId && o.KullaniciId == currentUserID);

            return View(anket);
        }

        // === 3. KATILIMCI: OY VERME ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int anketId, int secenekId)
        {
            var anket = await _context.Ankets.FindAsync(anketId);

            if (anket == null || anket.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Bu anket süresi dolduğu için oy verilemez.";
                return RedirectToAction(nameof(Poll));
            }

            var currentUserID = "test-kullanicisi";

            bool hasVoted = await _context.Oys
                // DÜZELTME: AnketID -> AnketId, UserID -> KullaniciId
                .AnyAsync(o => o.AnketId == anketId && o.KullaniciId == currentUserID);

            if (hasVoted)
            {
                TempData["Error"] = "Bu ankete zaten oy verdiniz.";
                return RedirectToAction(nameof(Poll));
            }

            var oy = new Oy
            {
                // DÜZELTME: Alan adları
                AnketId = anketId,
                SecenekId = secenekId,
                KullaniciId = currentUserID,

                // HATA GİDERİLDİ: İlişki adları
                Anket = null!,
                AnketSecenegi = null!
            };

            _context.Oys.Add(oy);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Oyunuz kaydedildi!";
            return RedirectToAction(nameof(Poll));
        }
    }
}