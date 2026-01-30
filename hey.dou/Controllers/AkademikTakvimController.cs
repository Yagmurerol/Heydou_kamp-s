using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models;

namespace hey.dou.Controllers
{
    public class AkademikTakvimController : Controller
    {
        private readonly HeydouContext _context;

        public AkademikTakvimController(HeydouContext context) // Veritabanı bağlamını başlatır
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(int? year, int? month) // Takvim ana sayfasını oluşturur ve aylık etkinlikleri listeler
        {
            try
            {
                var today = DateTime.Today;
                int currentYear = year ?? today.Year;
                int currentMonth = month ?? today.Month;

                var firstDay = new DateTime(currentYear, currentMonth, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                var events = await _context.AkademikTakvims
                    .Where(e =>
                        e.BaslangicTarihi <= DateOnly.FromDateTime(lastDay) &&
                        e.BitisTarihi >= DateOnly.FromDateTime(firstDay))
                    .OrderBy(e => e.BaslangicTarihi)
                    .ToListAsync();

                ViewBag.CurrentYear = currentYear;
                ViewBag.CurrentMonth = currentMonth;
                ViewBag.PreviousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                ViewBag.PreviousYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                ViewBag.NextMonth = currentMonth == 12 ? 1 : currentMonth + 1;
                ViewBag.NextYear = currentMonth == 12 ? currentYear + 1 : currentYear;

                return View(events);
            }
            catch
            {
                return View(new List<AkademikTakvim>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsByDate(string date) // MVC için seçilen tarihe özel etkinlikleri HTML olarak döndürür
        {
            try
            {
                if (!DateOnly.TryParse(date, out var selectedDate))
                    return Content("<p class='text-sm text-red-500'>Geçersiz tarih formatı</p>", "text/html");

                var today = DateOnly.FromDateTime(DateTime.Today);

                var events = await _context.AkademikTakvims
                    .Where(e => e.BaslangicTarihi <= selectedDate && e.BitisTarihi >= selectedDate)
                    .OrderBy(e => e.BaslangicTarihi)
                    .ToListAsync();

                if (!events.Any())
                {
                    events = await _context.AkademikTakvims
                        .Where(e => e.BaslangicTarihi >= today)
                        .OrderBy(e => e.BaslangicTarihi)
                        .Take(5)
                        .ToListAsync();
                }

                if (!events.Any())
                    return Content("<p class='text-sm text-slate-400'>Yaklaşan etkinlik bulunamadı.</p>", "text/html");

                var html = string.Join("", events.Select(e =>
                {
                    var baslangic = e.BaslangicTarihi.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
                    var bitis = e.BitisTarihi.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
                    var dateRange = baslangic == bitis ? baslangic : $"{baslangic} - {bitis}";
                    var kategori = string.IsNullOrEmpty(e.Kategori) ? "Etkinlik" : e.Kategori;

                    return $@"
                        <div class='flex items-start gap-3 p-4 bg-white dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-700 hover:shadow-md transition-shadow'>
                            <div class='flex-shrink-0 w-12 h-12 rounded-lg bg-primary/10 flex items-center justify-center'>
                                <span class='material-symbols-rounded text-primary'>event</span>
                            </div>
                            <div class='flex-1 min-w-0'>
                                <p class='font-semibold text-slate-900 dark:text-white mb-1'>{kategori}</p>
                                <p class='text-sm text-slate-500 dark:text-slate-400'>{dateRange}</p>
                                {(string.IsNullOrEmpty(e.Aciklama) ? "" : $"<p class='text-sm text-slate-600 dark:text-slate-300 mt-2'>{e.Aciklama}</p>")}
                            </div>
                        </div>";
                }));

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<p class='text-sm text-red-500'>Hata: {ex.Message}</p>", "text/html");
            }
        }

        [HttpGet("by-date")]
        [Route("api/akademiktakvim/by-date")]
        public async Task<ActionResult<ApiResponse<List<AkademikTakvim>>>> GetEventsByDateApi(string date) // API için seçilen tarihe özel etkinlikleri döndürür
        {
            try
            {
                if (!DateOnly.TryParse(date, out var selectedDate))
                    return BadRequest(new ApiResponse(false, "Geçersiz tarih formatı", 400));

                var today = DateOnly.FromDateTime(DateTime.Today);

                var events = await _context.AkademikTakvims
                    .Where(e => e.BaslangicTarihi <= selectedDate && e.BitisTarihi >= selectedDate)
                    .OrderBy(e => e.BaslangicTarihi)
                    .ToListAsync();

                if (!events.Any())
                {
                    events = await _context.AkademikTakvims
                        .Where(e => e.BaslangicTarihi >= today)
                        .OrderBy(e => e.BaslangicTarihi)
                        .Take(5)
                        .ToListAsync();

                    if (!events.Any())
                        return NotFound(new ApiResponse(false, "Yaklaşan etkinlik bulunamadı", 404));
                }

                return Ok(new ApiResponse<List<AkademikTakvim>>(true, "Etkinlikler getirildi", events));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost]
        [Route("api/akademiktakvim")]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] AkademikTakvim etkinlik) // Yeni akademik takvim etkinliği ekler
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse(false, "Geçersiz veriler", 400));

                _context.AkademikTakvims.Add(etkinlik);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Index), null, new ApiResponse(true, "Akademik takvim etkinliği başarıyla oluşturuldu", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }
    }
}