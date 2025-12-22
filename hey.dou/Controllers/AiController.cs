using hey.dou.Models;
using hey.dou.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hey.dou.Controllers
{
    public class AiDanismanController : Controller
    {
        private readonly IAiDanismanService _aiService;

        public AiDanismanController(IAiDanismanService aiService) // Yapay zeka servisini başlatır
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index() // AI Danışman sayfasını görüntüler ve oturum kontrolü yapar
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new AiDanismanViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AiDanismanViewModel model) // Kullanıcının sorusunu alır ve AI servisinden cevap döndürür
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(model.Question))
            {
                model.Error = "Lütfen bir soru yaz.";
                return View(model);
            }

            try
            {
                model.Answer = await _aiService.GetAnswerAsync(model.Question);
            }
            catch
            {
                model.Error = "Cevap alınırken bir hata oluştu. Lütfen daha sonra tekrar dene.";
            }

            return View(model);
        }
    }
}