using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace hey.dou.Controllers
{
    public class AccountController : Controller
    {
        private readonly HeydouContext _context;

        public AccountController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() // Giriş sayfasını görüntüler
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentLogin(string email, string password) // Öğrenci ve Kulüp Başkanı girişini yönetir
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login");
            }

            var kullanici = await _context.Kullanicilars.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                (k.Rol == "Ogrenci" || k.Rol == "KulupBaskani"));

            if (kullanici == null)
            {
                ViewBag.Error = "E-posta, şifre hatalı veya yetkiniz yok.";
                return View("Login");
            }

            // Oturum bilgilerini kaydeder
            HttpContext.Session.SetInt32("UserId", kullanici.KullaniciId);
            HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad ?? "Öğrenci");
            HttpContext.Session.SetString("Rol", kullanici.Rol ?? "Ogrenci");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> StaffLogin(string email, string password) // Personel girişini yönetir
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "E-posta ve şifre boş bırakılamaz.";
                return View("Login");
            }

            var kullanici = await _context.Kullanicilars.FirstOrDefaultAsync(k =>
                k.Email == email &&
                k.Sifre == password &&
                (k.Rol != "Ogrenci" && k.Rol != "KulupBaskani"));

            if (kullanici == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View("Login");
            }

            // Oturum bilgilerini kaydeder
            HttpContext.Session.SetInt32("UserId", kullanici.KullaniciId);
            HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad ?? "Personel");
            HttpContext.Session.SetString("Rol", kullanici.Rol ?? "Personel");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout() //  oturumu sonlandırır ve temizler
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}