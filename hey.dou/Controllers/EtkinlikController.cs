using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtkinlikController : ControllerBase
    {
        private readonly HeydouContext _context;

        public EtkinlikController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Event>>>> GetAll() // Anket sonucu seçilen aktif etkinlikleri listeler
        {
            try
            {
                var secilenEtkinlikler = await _context.Events
                                                   .Where(e => e.AnketSonucuSecildi.GetValueOrDefault() == true)
                                                   .ToListAsync();

                return Ok(new ApiResponse<List<Event>>(true, "Etkinlikler başarıyla getirildi", secilenEtkinlikler));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Event>>> GetById(int id) // ID'ye göre etkinlik getirir
        {
            try
            {
                var etkinlik = await _context.Events.FindAsync(id);

                if (etkinlik == null)
                    return NotFound(new ApiResponse(false, "Etkinlik bulunamadı", 404));

                return Ok(new ApiResponse<Event>(true, "Etkinlik başarıyla getirildi", etkinlik));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost("katilim")]
        public async Task<ActionResult<ApiResponse>> SaveParticipation(int etkinlikId, bool katiliyor) // Kullanıcının bir etkinliğe katılım durumunu kaydeder
        {
            try
            {
                var userIdHeader = Request.Headers["UserId"].ToString();
                if (!int.TryParse(userIdHeader, out int kullaniciId))
                {
                    return BadRequest(new ApiResponse(false, "Geçersiz Kullanıcı ID", 400));
                }

                var etkinlik = await _context.Events.FindAsync(etkinlikId);
                if (etkinlik == null)
                    return NotFound(new ApiResponse(false, "Etkinlik bulunamadı", 404));

                var katilimKaydi = await _context.Katilimlars
                                                 .FirstOrDefaultAsync(k => k.EtkinlikId == etkinlikId && k.KullaniciId == kullaniciId);

                if (katilimKaydi == null)
                {
                    katilimKaydi = new Katilim
                    {
                        EtkinlikId = etkinlikId,
                        KullaniciId = kullaniciId,
                        KatilimDurumu = katiliyor,
                        Etkinlik = null!
                    };
                    _context.Katilimlars.Add(katilimKaydi);
                }
                else
                {
                    katilimKaydi.KatilimDurumu = katiliyor;
                    _context.Katilimlars.Update(katilimKaydi);
                }

                await _context.SaveChangesAsync();
                return Ok(new ApiResponse(true, "Katılım durumu kaydedildi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet("{id}/katilimcilar")]
        public async Task<ActionResult<ApiResponse<List<Katilim>>>> GetParticipants(int id) // Belirli bir etkinliğe katılacağını bildiren kullanıcıların listesini döner
        {
            try
            {
                var etkinlik = await _context.Events.FindAsync(id);

                if (etkinlik == null || etkinlik.KatilimOylamasiAcik.GetValueOrDefault() == false)
                {
                    return NotFound(new ApiResponse(false, "Etkinlik bulunamadı veya katılım oylaması kapalı", 404));
                }

                var katilimcilar = await _context.Katilimlars
                                                 .Where(k => k.EtkinlikId == id && k.KatilimDurumu.GetValueOrDefault() == true)
                                                 .ToListAsync();

                return Ok(new ApiResponse<List<Katilim>>(true, $"{etkinlik.Title} - Katılımcılar", katilimcilar));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }
    }
}