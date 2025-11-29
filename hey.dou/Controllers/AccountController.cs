using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // Modellerin ve Context'in olduğu yer
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Session işlemleri için gerekli

namespace hey.dou.Controllers
{
    public class AccountController : Controller
    {
        private readonly HeydouContext _context;

        public AccountController(HeydouContext context)
        {
            _context = context;
        }

        // 1. Login Sayfasını Göster
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. ÖĞRENCİ GİRİŞİ (Kulüp Başkanı da buradan girecek)
        [HttpPost]
        public async Task<IActionResult> StudentLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login");
            }

            // DÜZELTME: Rolü 'Ogrenci' OLANLAR VEYA 'KulupBaskani' OLANLAR giriş yapabilir
            var kullanici = await _context.Kullanicilars.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                (k.Rol == "Ogrenci" || k.Rol == "KulupBaskani"));

            if (kullanici == null)
            {
                ViewBag.Error = "E-posta, şifre hatalı veya yetkiniz yok.";
                return View("Login");
            }

            // --- Kullanıcıyı Hafızaya (Session) At ---
            HttpContext.Session.SetInt32("UserId", kullanici.KullaniciId);
            HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad ?? "Öğrenci");

            // ÖNEMLİ: "Ogrenci" diye elle yazmıyoruz, veritabanındaki gerçek rolü (KulupBaskani ise onu) kaydediyoruz.
            // Bu sayede AnketController kimin başkan olduğunu anlayacak.
            HttpContext.Session.SetString("Rol", kullanici.Rol ?? "Ogrenci");
            // -----------------------------------------

            // Başarılı giriş -> Ana Sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

        // 3. PERSONEL GİRİŞİ
        [HttpPost]
        public async Task<IActionResult> StaffLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login");
            }

            // Personel girişi (Öğrenci ve Başkan olmayanlar)
            var kullanici = await _context.Kullanicilars.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                (k.Rol != "Ogrenci" && k.Rol != "KulupBaskani"));

            if (kullanici == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View("Login");
            }

            // --- Kullanıcıyı Hafızaya (Session) At ---
            HttpContext.Session.SetInt32("UserId", kullanici.KullaniciId);
            HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad ?? "Personel");
            HttpContext.Session.SetString("Rol", kullanici.Rol ?? "Personel");
            // -----------------------------------------

            // Başarılı giriş -> Ana Sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

        // 4. ÇIKIŞ YAP (Logout)
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Hafızayı temizle
            return RedirectToAction("Login");
        }
    }
}