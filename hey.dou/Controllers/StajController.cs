using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
using System.Linq;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
    // Artık normal MVC controller
    public class StajController : Controller
    {
        private readonly HeydouContext _context;

        public StajController(HeydouContext context)
        {
            _context = context;
        }

        // /Staj veya /Staj/Index → View
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // /api/Staj/Listele → JSON (frontend buraya istek atıyor)
        [HttpGet("api/Staj/Listele")]
        public async Task<IActionResult> Listele()
        {
            // Önce tüm kayıtları al (debug için filtreyi hafif tuttum)
            var sorgu = _context.StajIlanlaris.AsQueryable();

            // Aktif kolonun 1 olduğu (true) ilanlar gelsin,
            // ama null ise de göster (hocanın eklediği datalar kaçmasın diye)
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
