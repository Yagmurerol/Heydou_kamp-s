using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // HeydouContext ve Modeller burada
using System.Linq;
using System.Threading.Tasks;

// Namespace'i kendi projenize göre kontrol edin
namespace hey.dou.Controllers
{
    // 'ControllerBase' DEĞİL, 'Controller' olmalı
    public class AkademikTakvimController : Controller
    {
        // 1. Veritabanı köprüsünü (HeydouContext) çağır
        private readonly HeydouContext _context;

        // 2. Constructor: Köprüyü projeden iste
        public AkademikTakvimController(HeydouContext context)
        {
            _context = context;
        }

        // 3. /akademiktakvim adresine gidince burası çalışacak
        public async Task<IActionResult> Index()
        {
            // 4. Veritabanına bağlan ve 'AkademikTakvims' tablosundan veriyi çek
            // (HeydouContext.cs koduna göre tablonun adı 'AkademikTakvims')
            var etkinlikListesi = await _context.AkademikTakvims
                                        .OrderBy(e => e.BaslangicTarihi)
                                        .ToListAsync();

            // 5. Veriyi (etkinlikListesi) View'a (Index.cshtml) gönder
            return View(etkinlikListesi);
        }
    }
}