using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace hey.dou.Controllers
{
    public class KrokiController : Controller
    {
        private readonly HeydouContext _context;

        public KrokiController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }
        [ApiExplorerSettings(IgnoreApi = true)]        public async Task<IActionResult> Index() // Tüm mekanları listeler
        {
            try
            {
                var mekanlar = await _context.Mekanlars.Include(m => m.Kat).ToListAsync();
                return View(mekanlar);
            }
            catch
            {
                return View(new List<Mekanlar>());
            }
        }

        [HttpGet("{kod}")]
        [Route("api/kroki/{kod}")]
        public async Task<ActionResult<ApiResponse<MekanSonucModel>>> MekanAra(string kod) // Girilen mekan koduna göre veritabanında arama yapar ve kat/kroki bilgisini döner
        {
            try
            {
                if (string.IsNullOrEmpty(kod))
                {
                    return BadRequest(new ApiResponse(false, "Lütfen bir mekan kodu girin.", 400));
                }

                var mekan = await _context.Mekanlars
                                    .Include(m => m.Kat)
                                    .FirstOrDefaultAsync(x => x.MekanKodu == kod);

                if (mekan == null)
                {
                    return NotFound(new ApiResponse(false, "Bu koda sahip bir yer bulunamadı. Lütfen 'B2-01' gibi tam kod giriniz.", 404));
                }

                var sonuc = new MekanSonucModel
                {
                    MekanKodu = mekan.MekanKodu,
                    Aciklama = mekan.Aciklama,
                    KatAdi = mekan.Kat.KatAdi,
                    KrokiResimAdi = mekan.Kat.KrokiResimAdi
                };

                return Ok(new ApiResponse<MekanSonucModel>(true, "Mekan bulundu", sonuc));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet]
        [Route("api/kroki")]
        public async Task<ActionResult<ApiResponse<List<Mekanlar>>>> GetAll() // Tüm mekanları listeler
        {
            try
            {
                var mekanlar = await _context.Mekanlars
                    .Include(m => m.Kat)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Mekanlar>>(true, "Mekanlar başarıyla getirildi", mekanlar));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }
    }
}