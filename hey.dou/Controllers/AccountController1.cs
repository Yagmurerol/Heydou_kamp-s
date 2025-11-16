using Microsoft.AspNetCore.Mvc;

namespace hey.dou.Controllers // (veya sizin namespace'iniz)
{
    public class AccountController : Controller
    {
        // Proje çalıştığında /account/login adresine
        // veya ana adrese (Program.cs'ten dolayı) gidilince
        // bu metot çalışacak.
        public IActionResult Login()
        {
            // Views/Account/Login.cshtml dosyasını
            // tarayıcıya gönderir.
            return View();
        }

        // NOT: Backend ekibiniz daha sonra bu iki metodu
        // veritabanı girişi için dolduracak.

        [HttpPost]
        public IActionResult StudentLogin(string username, string password)
        {
            // Burası backend'in işi (Öğrenci girişi kontrolü)
            return RedirectToAction("Login"); // Şimdilik Login'e geri dönsün
        }

        [HttpPost]
        public IActionResult StaffLogin(string username, string password)
        {
            // Burası backend'in işi (Personel girişi kontrolü)
            return RedirectToAction("Login"); // Şimdilik Login'e geri dönsün
        }
    }
}