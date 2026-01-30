using System;
using System.Linq;
using System.Threading.Tasks;
using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace hey.dou.Controllers
{
    public class AnketController : Controller
    {
        private readonly HeydouContext _context;

        public AnketController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        private async Task<Anket?> GetActivePollAsync() // Süresi dolmamış en güncel aktif anketi getirir
        {
            return await _context.Ankets
                .Where(a => a.IsActive && a.EndDate > DateTime.Now)
                .OrderByDescending(a => a.AnketId)
                .FirstOrDefaultAsync();
        }

        private async Task<Anket?> GetPollWithDetailsAsync(int id) // Anketi, seçenekleri ve oylarıyla birlikte detaylıca getirir
        {
            return await _context.Ankets
                .Include(a => a.AnketSecenegis)
                    .ThenInclude(s => s.Oys)
                .FirstOrDefaultAsync(m => m.AnketId == id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index() // Tüm anketleri listeler
        {
            try
            {
                var anketler = await _context.Ankets.OrderByDescending(a => a.AnketId).ToListAsync();
                return View(anketler);
            }
            catch
            {
                return View(new List<Anket>());
            }
        }

        [HttpGet]
        [Route("api/anket")]
        public async Task<ActionResult<ApiResponse<List<Anket>>>> GetAll() // Tüm anketleri listeler
        {
            try
            {
                var anketler = await _context.Ankets.OrderByDescending(a => a.AnketId).ToListAsync();
                return Ok(new ApiResponse<List<Anket>>(true, "Anketler başarıyla getirildi", anketler));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet("active")]
        [Route("api/anket/active")]
        public async Task<ActionResult<ApiResponse<Anket>>> GetActivePoll() // Aktif anketi getirir
        {
            try
            {
                var activePoll = await GetActivePollAsync();
                if (activePoll == null)
                    return NotFound(new ApiResponse(false, "Aktif anket bulunamadı", 404));

                return Ok(new ApiResponse<Anket>(true, "Aktif anket getirildi", activePoll));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet("{id}")]
        [Route("api/anket/{id}")]
        public async Task<ActionResult<ApiResponse<Anket>>> GetById(int? id) // ID'ye göre anket detaylarını getirir
        {
            try
            {
                Anket? anket = (id == null)
                    ? await GetActivePollAsync()
                    : await GetPollWithDetailsAsync(id.Value);

                if (anket == null)
                    return NotFound(new ApiResponse(false, "Anket bulunamadı", 404));

                if (id == null) anket = await GetPollWithDetailsAsync(anket.AnketId);

                return Ok(new ApiResponse<Anket>(true, "Anket detayları getirildi", anket));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost("vote")]
        [Route("api/anket/vote")]
        public async Task<ActionResult<ApiResponse>> Vote([FromBody] VoteRequest request) // Oya basılması işlemini gerçekleştirir
        {
            try
            {
                var userIdHeader = Request.Headers["UserId"].ToString();
                if (!int.TryParse(userIdHeader, out int kullaniciId))
                    return BadRequest(new ApiResponse(false, "Geçersiz Kullanıcı ID", 400));

                string currentUserID = kullaniciId.ToString();

                var anket = await _context.Ankets.FindAsync(request.AnketId);
                if (anket == null || anket.EndDate < DateTime.Now)
                    return BadRequest(new ApiResponse(false, "Anket bulunamadı veya süresi dolmuş", 400));

                bool hasVoted = await _context.Oys.AnyAsync(o => o.AnketId == request.AnketId && o.KullaniciId == currentUserID);
                if (hasVoted)
                    return BadRequest(new ApiResponse(false, "Zaten oy vermişsiniz", 400));

                _context.Oys.Add(new Oy { AnketId = request.AnketId, SecenekId = request.SecenekId, KullaniciId = currentUserID });
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Oy başarıyla kaydedildi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        private List<AnketSecenegi> GetDefaultOptions(int anketId) // Yeni anketler için sistem tarafından oluşturulan hazır seçenekler
        {
            return new List<AnketSecenegi>
            {
                new AnketSecenegi { AnketId = anketId, SecenekText = "Hafta Sonu Doğa Yürüyüşü", KulupAdi = "Gezi Kulübü", TarihSaat = DateTime.Now.AddDays(3) },
                new AnketSecenegi { AnketId = anketId, SecenekText = "Kutu Oyunu Gecesi", KulupAdi = "Oyun Kulübü", TarihSaat = DateTime.Now.AddDays(2) }
            };
        }
    }
}