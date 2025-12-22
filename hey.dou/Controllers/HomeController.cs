using System.Diagnostics;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;

namespace hey.dou.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) // Loglama servisini baþlatýr
        {
            _logger = logger;
        }

        public IActionResult Index() // Uygulamanýn ana sayfasýný görüntüler
        {
            return View();
        }

        public IActionResult Privacy() // Gizlilik politikasý sayfasýný görüntüler
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() // Uygulama genelinde oluþan hatalarý yakalar ve hata sayfasýný görüntüler
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}