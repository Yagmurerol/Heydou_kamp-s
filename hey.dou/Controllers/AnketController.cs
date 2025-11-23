using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // Session işlemleri için şart

namespace hey.dou.Controllers
{
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;

        public AnketController(HeydouContext context)
        {
            _context = context;
        }

        // Yardımcı metot 1: Aktif, süresi dolmamış anketi getirir
        private async Task<Anket?> GetActivePollAsync()
        {
            return await _context.Ankets
                .Where(a => a.IsActive && a.EndDate > DateTime.Now)
                .OrderByDescending(a => a.AnketId)
                .FirstOrDefaultAsync();
        }

        // Yardımcı metot 2: En son anketi getir (detaylar için)
        private async Task<Anket?> GetCurrentPollAsync()
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                .OrderByDescending(a => a.AnketId)
                .FirstOrDefaultAsync();
        }

        // === 1. GİRİŞ KAPISI / LİSTELEME (Index) - TEK METHOD ===
        // Bu metot, Öğrenciyi direkt Details'a, Başkanı liste/oluşturma sayfasına yönlendirir.
        public async Task<IActionResult> Index()
        {
            // Giriş kontrolü
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            var userRole = HttpContext.Session.GetString("Rol");
            var activePoll = await GetActivePollAsync();

            // 1. ÖĞRENCİ İSE VE ANKET VARSA DİREKT OYLAMA EKRANINA AT
            if (userRole != "KulupBaskani" && activePoll != null)
            {
                return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
            }

            // 2. KULÜP BAŞKANI İSE VEYA ANKET YOKSA LİSTEYİ GÖSTER
            var anketler = await _context.Ankets.OrderByDescending(a => a.AnketId).ToListAsync();

            // Eğer öğrenci ise ve anket yoksa hata mesajı ver
            if (userRole != "KulupBaskani" && anketler.Count == 0)
            {
                TempData["Info"] = "Şu anda oy verebileceğiniz aktif bir anket bulunmamaktadır.";
            }

            return View(anketler);
        }

        // === 2. KULÜP BAŞKANI: SEÇENEK EKLEME SAYFASI (Create - GET) ===
        public async Task<IActionResult> Create()
        {
            // Giriş ve Yetki Kontrolü
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (HttpContext.Session.GetString("Rol") != "KulupBaskani" || userIdFromSession == null)
            {
                TempData["Error"] = "Bu sayfaya sadece Kulüp Başkanları etkinlik ekleyebilir.";
                return RedirectToAction(nameof(Index));
            }

            var activePoll = await GetActivePollAsync();

            if (activePoll == null)
            {
                TempData["Error"] = "Şu anda oy verme süresi aktif olan bir ana anket bulunmamaktadır.";
                return RedirectToAction(nameof(Index));
            }

            // Kısıtlama Kontrolü
            string currentUserID = userIdFromSession.Value.ToString();
            bool alreadyProposed = await _context.AnketSecenegis
                .AnyAsync(s => s.AnketId == activePoll.AnketId && s.CreatorKullaniciId == currentUserID);

            if (alreadyProposed)
            {
                TempData["Info"] = "Öneriniz zaten kaydedilmiştir. Oylama ekranına yönlendiriliyorsunuz.";
                return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
            }

            ViewBag.ActivePollId = activePoll.AnketId;
            return View();
        }

        // === 3. SEÇENEK KAYDETME İŞLEMİ (Create - POST) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int activePollId, string proposedTitle)
        {
            // Güvenlik Kontrolü
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (HttpContext.Session.GetString("Rol") != "KulupBaskani" || userIdFromSession == null) return RedirectToAction("Login", "Account");

            string currentUserID = userIdFromSession.Value.ToString();

            if (string.IsNullOrWhiteSpace(proposedTitle))
            {
                TempData["Error"] = "Öneri başlığı boş bırakılamaz.";
                ViewBag.ActivePollId = activePollId;
                return View();
            }

            // Kısıtlama Kontrolü (Tekrar)
            bool alreadyProposed = await _context.AnketSecenegis
                .AnyAsync(s => s.AnketId == activePollId && s.CreatorKullaniciId == currentUserID);

            if (alreadyProposed)
            {
                TempData["Error"] = "Bu ankete zaten bir etkinlik önerisi eklediniz.";
                return RedirectToAction(nameof(Details), new { id = activePollId });
            }

            // Yeni seçeneği oluştur ve Creator ID'sini kaydet
            var newOption = new AnketSecenegi
            {
                AnketId = activePollId,
                SecenekText = proposedTitle,
                CreatorKullaniciId = currentUserID
            };

            _context.AnketSecenegis.Add(newOption);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Yeni etkinlik önerisi ('{proposedTitle}') başarıyla ankete eklendi!";

            // BAŞARILI YÖNLENDİRME
            return RedirectToAction(nameof(Details), new { id = activePollId });
        }


        // === 4. ANKET DETAYI / OYLAMA (Details) ===
        public async Task<IActionResult> Details(int? id)
        {
            // Giriş Kontrolü
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null) return RedirectToAction("Login", "Account");

            string currentUserID = userIdFromSession.Value.ToString();

            // Anket yüklemesi
            var anket = (id == null)
                ? await GetActivePollAsync()
                : await _context.Ankets
                            .Include(a => a.AnketSecenegis)
                            .ThenInclude(s => s.Oys)
                            .FirstOrDefaultAsync(m => m.AnketId == id);

            if (anket == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Oy kontrolü
            ViewBag.HasVoted = await _context.Oys
                .AnyAsync(o => o.AnketId == anket.AnketId && o.KullaniciId == currentUserID);

            return View(anket);
        }

        // === 5. OY VERME İŞLEMİ (Vote) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int anketId, int secenekId)
        {
            // Güvenlik Kontrolü
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null) return RedirectToAction("Login", "Account");

            string currentUserID = userIdFromSession.Value.ToString();

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
                KullaniciId = currentUserID
            };

            _context.Oys.Add(oy);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Oyunuz kaydedildi!";
            return RedirectToAction(nameof(Details), new { id = anketId });
        }

        // === 6. LİSTELEME (Index) - Artık ana giriş kapısı değil ===
        
    }
}