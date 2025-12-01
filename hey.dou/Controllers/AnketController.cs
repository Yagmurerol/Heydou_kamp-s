using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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

        // Yardımcı metot 2: Detaylı anket getir
        private async Task<Anket?> GetPollWithDetailsAsync(int id)
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                .FirstOrDefaultAsync(m => m.AnketId == id);
        }

        // === 1. INDEX (YÖNLENDİRME MERKEZİ) ===
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            var userRole = HttpContext.Session.GetString("Rol");
            var activePoll = await GetActivePollAsync();

            // SENARYO 1: Öğrenci ise ve aktif anket varsa -> DİREKT OYLAMAYA GİT
            if (userRole != "KulupBaskani" && activePoll != null)
            {
                return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
            }

            // SENARYO 2: Kulüp Başkanı ise veya anket yoksa -> LİSTEYİ GÖSTER
            var anketler = await _context.Ankets.OrderByDescending(a => a.AnketId).ToListAsync();

            if (userRole != "KulupBaskani" && anketler.Count == 0)
            {
                TempData["Info"] = "Şu anda oy verebileceğiniz aktif bir anket bulunmamaktadır.";
            }

            return View(anketler);
        }

        // === 2. CREATE (GET - SAYFAYI AÇ) ===
        public async Task<IActionResult> Create()
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (HttpContext.Session.GetString("Rol") != "KulupBaskani" || userIdFromSession == null)
            {
                TempData["Error"] = "Bu sayfaya sadece Kulüp Başkanları etkinlik ekleyebilir.";
                return RedirectToAction(nameof(Index));
            }

            string currentUserID = userIdFromSession.Value.ToString();
            var activePoll = await GetActivePollAsync();

            // Kısıtlama Kontrolü: Bu başkan zaten öneri eklemiş mi?
            if (activePoll != null)
            {
                bool alreadyProposed = await _context.AnketSecenegis
                    .AnyAsync(s => s.AnketId == activePoll.AnketId && s.CreatorKullaniciId == currentUserID);

                if (alreadyProposed)
                {
                    TempData["Info"] = "Öneriniz zaten kaydedilmiştir. Oylama ekranına yönlendiriliyorsunuz.";
                    return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
                }

                ViewBag.ActivePollId = activePoll.AnketId;
            }

            return View(new EtkinlikCreateViewModel());
        }

        // === 3. CREATE (POST - ÖNERİYİ KAYDET) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EtkinlikCreateViewModel model)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (HttpContext.Session.GetString("Rol") != "KulupBaskani" || userIdFromSession == null)
                return RedirectToAction("Login", "Account");

            string currentUserID = userIdFromSession.Value.ToString();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isNewPoll = false; // Yeni anket mi oluşturuyoruz kontrolü

            // 1. Aktif Anket Var mı Kontrol Et
            var activePoll = await GetActivePollAsync();

            if (activePoll == null)
            {
                // YOKSA YENİ ANA ANKET OLUŞTUR
                isNewPoll = true; // Evet, bu yeni bir anket

                activePoll = new Anket
                {
                    Title = "Haftanın Etkinlik Oylaması",
                    Description = "Kulüp başkanlarının önerdiği etkinliklerden birini seçin.",
                    EndDate = DateTime.Now.AddDays(1),
                    IsActive = true
                };
                _context.Ankets.Add(activePoll);
                await _context.SaveChangesAsync(); // ID oluştu
            }
            else
            {
                // VARSA KISITLAMA KONTROLÜ YAP
                bool alreadyProposed = await _context.AnketSecenegis
                    .AnyAsync(s => s.AnketId == activePoll.AnketId && s.CreatorKullaniciId == currentUserID);

                if (alreadyProposed)
                {
                    return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
                }
            }

            // 2. Başkanın Girdiği Seçeneği Ekle
            var baskaninOnerisi = new AnketSecenegi
            {
                AnketId = activePoll.AnketId,
                SecenekText = model.EtkinlikAdi,
                CreatorKullaniciId = currentUserID,

                KulupAdi = model.KulupAdi,
                KulupBaskaniAdi = model.KulupBaskaniAdi,
                Konum = model.Konum,
                TarihSaat = model.TarihSaat,
                KisaAciklama = model.KisaAciklama
            };
            _context.AnketSecenegis.Add(baskaninOnerisi);

            // 3. EĞER BU YENİ BİR ANKET İSE -> OTOMATİK SEÇENEKLERİ DE EKLE
            if (isNewPoll)
            {
                var otomatikSecenekler = new List<AnketSecenegi>
                {
                    new AnketSecenegi
                    {
                        AnketId = activePoll.AnketId,
                        SecenekText = "Hafta Sonu Doğa Yürüyüşü",
                        KulupAdi = "Gezi ve Kampçılık Kulübü",
                        KulupBaskaniAdi = "Mert Demir",
                        Konum = "Belgrad Ormanı / Neşet Suyu",
                        TarihSaat = DateTime.Now.AddDays(3).AddHours(9),
                        KisaAciklama = "Doğayla iç içe, oksijen dolu harika bir yürüyüş etkinliği. Spor ayakkabılarınızı ve suyunuzu getirmeyi unutmayın."
                    },
                    new AnketSecenegi
                    {
                        AnketId = activePoll.AnketId,
                        SecenekText = "Kutu Oyunu Gecesi",
                        KulupAdi = "Oyun ve Strateji Kulübü",
                        KulupBaskaniAdi = "Zeynep Kaya",
                        Konum = "Kampüs Kafeterya",
                        TarihSaat = DateTime.Now.AddDays(2).AddHours(18),
                        KisaAciklama = "Catan, Monopoly, Tabu ve daha fazlası! Stratejini konuştur, eğlenceli bir rekabet gecesi seni bekliyor."
                    },
                    new AnketSecenegi
                    {
                        AnketId = activePoll.AnketId,
                        SecenekText = "Topluluk Gönüllülük Günü",
                        KulupAdi = "Sosyal Sorumluluk Kulübü",
                        KulupBaskaniAdi = "Elif Yılmaz",
                        Konum = "Merkez Kampüs Meydanı",
                        TarihSaat = DateTime.Now.AddDays(5).AddHours(13),
                        KisaAciklama = "Çevre temizliği ve sokak hayvanları için mama dağıtımı yapacağız. Hep birlikte dünyayı güzelleştirelim."
                    },
                    new AnketSecenegi
                    {
                        AnketId = activePoll.AnketId,
                        SecenekText = "Kodlama Atölyesi",
                        KulupAdi = "Yazılım ve İnovasyon Kulübü",
                        KulupBaskaniAdi = "Burak Çelik",
                        Konum = "Bilgisayar Lab. B Blok",
                        TarihSaat = DateTime.Now.AddDays(4).AddHours(15),
                        KisaAciklama = "Python ile Veri Analizine Giriş. Hiç bilmeyenler için temel seviyeden başlayacağız."
                    }
                };
                _context.AnketSecenegis.AddRange(otomatikSecenekler);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Etkinlik öneriniz ('{model.EtkinlikAdi}') oylamaya eklendi!";
            return RedirectToAction(nameof(Details), new { id = activePoll.AnketId });
        }

        // === 4. DETAILS (OYLAMA EKRANI) ===
        public async Task<IActionResult> Details(int? id)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null) return RedirectToAction("Login", "Account");
            string currentUserID = userIdFromSession.Value.ToString();

            Anket? anket;

            if (id == null)
            {
                anket = await GetActivePollAsync();
                if (anket != null) anket = await GetPollWithDetailsAsync(anket.AnketId);
            }
            else
            {
                anket = await GetPollWithDetailsAsync(id.Value);
            }

            if (anket == null) return RedirectToAction(nameof(Index));

            ViewBag.HasVoted = await _context.Oys
                .AnyAsync(o => o.AnketId == anket.AnketId && o.KullaniciId == currentUserID);

            return View(anket);
        }

        // === 5. VOTE (OY VERME) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int anketId, int secenekId)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession == null) return RedirectToAction("Login", "Account");
            string currentUserID = userIdFromSession.Value.ToString();

            var anket = await _context.Ankets.FindAsync(anketId);

            if (anket == null || anket.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Bu anket süresi dolduğu için oy verilemez.";
                return RedirectToAction(nameof(Details), new { id = anketId });
            }

            bool hasVoted = await _context.Oys.AnyAsync(o => o.AnketId == anketId && o.KullaniciId == currentUserID);

            if (hasVoted)
            {
                TempData["Error"] = "Zaten oy kullandınız.";
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
    }
}