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

        public async Task<IActionResult> Index(int? year, int? month) // Takvim ana sayfasını oluşturur ve aylık etkinlikleri listeler
        {
            var today = DateTime.Today;
            int currentYear = year ?? today.Year;
            int currentMonth = month ?? today.Month;

            var firstDay = new DateTime(currentYear, currentMonth, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var prev = firstDay.AddMonths(-1);
            var next = firstDay.AddMonths(1);

            ViewBag.CurrentYear = currentYear;
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.PrevYear = prev.Year;
            ViewBag.PrevMonth = prev.Month;
            ViewBag.NextYear = next.Year;
            ViewBag.NextMonth = next.Month;

            var events = await _context.AkademikTakvims
                .Where(e =>
                    e.BaslangicTarihi <= DateOnly.FromDateTime(lastDay) &&
                    e.BitisTarihi >= DateOnly.FromDateTime(firstDay))
                .OrderBy(e => e.BaslangicTarihi)
                .ToListAsync();

            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsByDate(string date) // Seçilen tarihe özel etkinlikleri dinamik HTML olarak döndürür
        {
            if (!DateOnly.TryParse(date, out var selectedDate))
                return Content("<p class='text-red-500 text-sm'>Geçersiz tarih.</p>", "text/html");

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
                    return Content("<p class='text-gray-400 text-sm'>Yaklaşan etkinlik bulunamadı.</p>", "text/html");
            }

            var html = string.Join("", events.Select(ev =>
            {
                var renk = string.IsNullOrWhiteSpace(ev.RenkKodu)
                    ? (ev.Kategori?.Contains("Sınav") == true ? "#f59e0b"
                    : ev.Kategori?.Contains("Tatil") == true ? "#10b981"
                    : ev.Kategori?.Contains("Kayıt") == true ? "#ef4444"
                    : "#94a3b8")
                    : ev.RenkKodu;

                return $@"
                    <div class='flex items-start gap-3 p-3 rounded-xl hover:bg-slate-50'>
                        <div class='h-3 w-3 rounded-full mt-1.5' style='background-color:{renk}'></div>
                        <div>
                            <p class='font-semibold text-sm'>{ev.Kategori}</p>
                            <p class='text-xs text-gray-500'>{ev.Aciklama}</p>
                            <p class='text-[11px] text-gray-400'>
                                {ev.BaslangicTarihi:dd.MM.yyyy} - {ev.BitisTarihi:dd.MM.yyyy}
                            </p>
                        </div>
                    </div>";
            }));

            return Content(html, "text/html");
        }
    }
}