using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // HeydouContext ve Kullanici modellerinin olduğu yer
using System.Linq;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
    public class AccountController : Controller
    {
        // 1. Veritabanı köprüsünü (HeydouContext) çağır
        private readonly HeydouContext _context;

        public AccountController(HeydouContext context)
        {
            _context = context;
        }

        // 2. /account/login adresine gidilince bu metot çalışır
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Account/Login.cshtml'i gösterir
        }

        // 3. ÖĞRENCİ GİRİŞ Formu bu metodu tetikler
        [HttpPost]
        public async Task<IActionResult> StudentLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login"); // Hata mesajıyla Login sayfasını geri göster
            }

            // Veritabanında eşleşen öğrenciyi ara
            // 'Kullanicilar' adının HeydouContext.cs dosyanızda tanımlı olduğunu varsayıyoruz
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                k.Rol == "Ogrenci"); // Rol'ün DB'de "Ogrenci" yazdığını varsayıyorum

            if (kullanici == null)
            {
                // Eşleşme bulunamazsa
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View("Login"); // Hata mesajıyla Login sayfasını geri göster
            }

            // GİRİŞ BAŞARILI!
            // Kullanıcıyı Dashboard'a (HomeController'ın Index'ine) yönlendir.
            return RedirectToAction("Index", "Home");
        }

        // 4. PERSONEL GİRİŞ Formu bu metodu tetikler
        [HttpPost]
        public async Task<IActionResult> StaffLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login");
            }

            // Veritabanında eşleşen personeli ara
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                (k.Rol != "Ogrenci")); // Rol'ü "Ogrenci" olmayan herkes (Personel, Admin vb.)

            if (kullanici == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View("Login");
            }

            // GİRİŞ BAŞARILI!
            // Kullanıcıyı Dashboard'a (HomeController'ın Index'ine) yönlendir.
            return RedirectToAction("Index", "Home");
        }
    }
} // <-- 82. SATIRDAKİ HATA MUHTEMELEN BU PARANTEZİN EKSİKLİĞİYDİ