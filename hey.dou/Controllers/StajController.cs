using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
using System.Linq;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
    public class StajController : Controller
    {
        private readonly HeydouContext _context;

        public StajController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() // Staj ilanlarının gösterileceği ana sayfa arayüzünü döner
        {
            return View();
        }

        [HttpGet("api/Staj/Listele")]
        public async Task<IActionResult> Listele() // Aktif staj ilanlarını veritabanından çekerek JSON formatında listeler
        {
            var sorgu = _context.StajIlanlaris.AsQueryable();

            sorgu = sorgu.Where(i => i.Aktif == null || i.Aktif == true);

            var ilanlar = await sorgu
                .OrderByDescending(i => i.YayinlanmaTarihi)
                .Select(i => new
                {
                    i.Id,
                    i.Baslik,
                    i.Sirket,
                    i.Lokasyon,
                    i.StajTuru,
                    i.Aciklama,
                    i.YayinlanmaTarihi,
                    i.SonBasvuru,
                    i.BasvuruLinki
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = ilanlar.Count,
                stajlar = ilanlar
            });
        }
    }
}