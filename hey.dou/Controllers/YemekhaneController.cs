using hey.dou.Models;
using HeyDOU.KampusApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeyDOU.KampusApp.Controllers
{
    public class YemekhaneController : Controller
    {
        private readonly HeydouContext _context;

        public YemekhaneController(HeydouContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? startOfWeek)
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

            // 1️⃣ SQL'den sadece bu haftanın kayıtlarını çek
            var allMenus = await _context.HaftalikMenus
                .Where(m => m.Tarih >= monday && m.Tarih <= friday)
                .OrderBy(m => m.Tarih)
                .ToListAsync();

            // 2️⃣ Bellekte gruplandır (EF Core bug fix)
            var menus = allMenus
                .GroupBy(m => m.Gun)
                .Select(g => g.OrderByDescending(x => x.Tarih).First())
                .OrderBy(m => m.Tarih)
                .ToList();

            // 3️⃣ Tarih bilgilerini ViewBag ile gönder
            ViewBag.Baslangic = monday;
            ViewBag.Bitis = friday;
            ViewBag.OncekiHafta = monday.AddDays(-7);
            ViewBag.SonrakiHafta = monday.AddDays(7);

            return View(menus);
        }

        public IActionResult MenuEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEkle(HaftalikMenu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        private static DateOnly GetMonday(DateOnly date)
        {
            int diff = date.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : DayOfWeek.Monday - date.DayOfWeek;

            return date.AddDays(diff);
        }
    }
}
