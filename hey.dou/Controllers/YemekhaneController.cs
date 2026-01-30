using hey.dou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hey.dou.Controllers
{
    public class YemekhaneController : Controller
    {
        private readonly HeydouContext _context;

        public YemekhaneController(HeydouContext context) // Veritabanı bağlantısını başlatır
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(string? startOfWeek) // Haftalık yemek menüsünü görüntüler
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                DateOnly monday;
                if (!string.IsNullOrWhiteSpace(startOfWeek) &&
                    DateOnly.TryParse(startOfWeek, out var parsed))
                {
                    monday = GetMonday(parsed);
                }
                else
                {
                    monday = GetMonday(today);
                }

                var friday = monday.AddDays(4);

                var allMenus = await _context.HaftalikMenus
                    .Where(m => m.Tarih >= monday && m.Tarih <= friday)
                    .OrderBy(m => m.Tarih)
                    .ToListAsync();

                var menus = allMenus
                    .GroupBy(m => m.Gun)
                    .Select(g => g.OrderByDescending(x => x.Tarih).First())
                    .OrderBy(m => m.Tarih)
                    .ToList();

                ViewBag.Baslangic = monday;
                ViewBag.Bitis = friday;
                ViewBag.OncekiHafta = monday.AddDays(-7);
                ViewBag.SonrakiHafta = monday.AddDays(7);

                return View(menus);
            }
            catch
            {
                return View(new List<HaftalikMenu>());
            }
        }

        [HttpGet]
        [Route("api/yemekhane")]
        public async Task<ActionResult<ApiResponse<List<HaftalikMenu>>>> GetMenus(string? startOfWeek) // Haftalık yemek menüsünü tarihe göre hesaplar ve listeler
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                DateOnly monday;
                if (!string.IsNullOrWhiteSpace(startOfWeek) &&
                    DateOnly.TryParse(startOfWeek, out var parsed))
                {
                    monday = GetMonday(parsed);
                }
                else
                {
                    monday = GetMonday(today);
                }

                var friday = monday.AddDays(4);

                var allMenus = await _context.HaftalikMenus
                    .Where(m => m.Tarih >= monday && m.Tarih <= friday)
                    .OrderBy(m => m.Tarih)
                    .ToListAsync();

                var menus = allMenus
                    .GroupBy(m => m.Gun)
                    .Select(g => g.OrderByDescending(x => x.Tarih).First())
                    .OrderBy(m => m.Tarih)
                    .ToList();

                return Ok(new ApiResponse<List<HaftalikMenu>>(true, "Haftalık menü başarıyla getirildi", menus));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost]
        [Route("api/yemekhane")]
        public async Task<ActionResult<ApiResponse>> AddMenu([FromBody] HaftalikMenu menu) // Yeni menü ekler
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse(false, "Geçersiz veriler", 400));

                _context.HaftalikMenus.Add(menu);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMenus), null, new ApiResponse(true, "Menü başarıyla eklendi", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPut("{id}")]
        [Route("api/yemekhane/{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateMenu(int id, [FromBody] HaftalikMenu menu) // Menü günceller
        {
            try
            {
                var mevcutMenu = await _context.HaftalikMenus.FindAsync(id);
                if (mevcutMenu == null)
                    return NotFound(new ApiResponse(false, "Menü bulunamadı", 404));

                mevcutMenu.Gun = menu.Gun;
                mevcutMenu.Corba = menu.Corba;
                mevcutMenu.AnaYemek = menu.AnaYemek;
                mevcutMenu.YardimciYemek = menu.YardimciYemek;
                mevcutMenu.TatliVeyaMeyve = menu.TatliVeyaMeyve;
                mevcutMenu.Tarih = menu.Tarih;

                _context.HaftalikMenus.Update(mevcutMenu);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Menü başarıyla güncellendi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpDelete("{id}")]
        [Route("api/yemekhane/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteMenu(int id) // Menü siler
        {
            try
            {
                var menu = await _context.HaftalikMenus.FindAsync(id);
                if (menu == null)
                    return NotFound(new ApiResponse(false, "Menü bulunamadı", 404));

                _context.HaftalikMenus.Remove(menu);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Menü başarıyla silindi", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        private static DateOnly GetMonday(DateOnly date) // Verilen bir tarihin dahil olduğu haftanın Pazartesi gününü bulur
        {
            int diff = date.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : DayOfWeek.Monday - date.DayOfWeek;

            return date.AddDays(diff);
        }
    }
}