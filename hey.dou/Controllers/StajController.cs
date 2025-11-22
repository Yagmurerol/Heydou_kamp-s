// USING'ler en üstte ve doğru sırada olmalı
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeyDou.Models;
using hey.dou.Models; // <-- YENİ CONTEXT NAMESPACE'İNİZ
using System.Linq;
using System.Threading.Tasks;

// NAMESPACE TANIMI
namespace HeyDou.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StajController : ControllerBase
    {
        // Context adını güncelledik
        private readonly HeydouContext _context;

        // Context adını güncelledik
        public StajController(HeydouContext context)
        {
            _context = context;
        }

        // GET: /api/Staj/Listele
        [HttpGet("Listele")]
        public async Task<IActionResult> GetStajIlanlari()
        {
            var ilanlar = await _context.StajIlanlari // StajIlanlari DbSet'ini kullanıyoruz
                                    .Where(i => i.Aktif)
                                    .OrderByDescending(i => i.YayinlanmaTarihi)
                                    .Take(50)
                                    .Select(i => new
                                    {
                                        i.Id,
                                        i.Baslik,
                                        i.Sirket,
                                        i.Lokasyon,
                                        i.StajTuru,
                                        i.Aciklama,
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