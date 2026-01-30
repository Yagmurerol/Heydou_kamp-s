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

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index() // Aktif staj ilanlarını veritabanından çekerek JSON formatında listeler
        {
            try
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

                return View(ilanlar);
            }
            catch
            {
                return View(new List<object>());
            }
        }

        [HttpGet]
        [Route("api/staj")]
        public async Task<ActionResult<ApiResponse<object>>> GetAll() // API için staj ilanlarını listeler
        {
            try
            {
                var ilanlar = await _context.StajIlanlaris
                    .Where(i => i.Aktif == null || i.Aktif == true)
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

                return Ok(new ApiResponse<object>(true, "Staj ilanları başarıyla getirildi", new { stajlar = ilanlar }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpGet("{id}")]
        [Route("api/staj/{id}")]
        public async Task<ActionResult<ApiResponse<StajIlanlari>>> GetById(int id) // ID'ye göre staj ilanını getirir
        {
            try
            {
                var ilan = await _context.StajIlanlaris.FirstOrDefaultAsync(i => i.Id == id);

                if (ilan == null)
                    return NotFound(new ApiResponse(false, "Staj ilanı bulunamadı", 404));

                return Ok(new ApiResponse<StajIlanlari>(true, "Staj ilanı başarıyla getirildi", ilan));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost]
        [Route("api/staj")]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] StajIlanlari ilan) // Yeni staj ilanı ekler
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse(false, "Geçersiz veriler", 400));

                _context.StajIlanlaris.Add(ilan);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = ilan.Id }, 
                    new ApiResponse(true, "Staj ilanı başarıyla oluşturuldu", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPut("{id}")]
        [Route("api/staj/{id}")]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] StajIlanlari ilan) // Staj ilanını günceller
        {
            try
            {
                var mevcutIlan = await _context.StajIlanlaris.FindAsync(id);
                if (mevcutIlan == null)
                    return NotFound(new ApiResponse(false, "Staj ilanı bulunamadı", 404));

                mevcutIlan.Baslik = ilan.Baslik;
                mevcutIlan.Sirket = ilan.Sirket;
                mevcutIlan.Lokasyon = ilan.Lokasyon;
                mevcutIlan.StajTuru = ilan.StajTuru;
                mevcutIlan.Aciklama = ilan.Aciklama;
                mevcutIlan.SonBasvuru = ilan.SonBasvuru;
                mevcutIlan.BasvuruLinki = ilan.BasvuruLinki;

                _context.StajIlanlaris.Update(mevcutIlan);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Staj ilanı başarıyla güncellendi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpDelete("{id}")]
        [Route("api/staj/{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id) // Staj ilanını siler
        {
            try
            {
                var ilan = await _context.StajIlanlaris.FindAsync(id);
                if (ilan == null)
                    return NotFound(new ApiResponse(false, "Staj ilanı bulunamadı", 404));

                _context.StajIlanlaris.Remove(ilan);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Staj ilanı başarıyla silindi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }
    }
}