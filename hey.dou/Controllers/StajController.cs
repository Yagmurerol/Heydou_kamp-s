using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // Modellerin olduğu yer
using System.Linq;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StajController : ControllerBase
	{
		private readonly HeydouContext _context;

		public StajController(HeydouContext context)
		{
			_context = context;
		}

		// GET: /api/Staj/Listele
		[HttpGet("Listele")]
		public async Task<IActionResult> GetStajIlanlari()
		{
			// NOT: '_context.StajIlanlaris' hata verirse sonundaki 's'yi silip '_context.StajIlanlari' yapın.
			// Scaffold işlemi genelde 's' takısı ekler.
			var ilanlar = await _context.StajIlanlaris
										// --- HATA DÜZELTME ---
										// Hatalı olan: .Where(i => i.Aktif)
										// Düzeltilmiş: .Where(i => i.Aktif.GetValueOrDefault() == true)
										// Açıklama: 'Aktif' alanı null gelirse false kabul et, sadece true olanları getir.
										.Where(i => i.Aktif.GetValueOrDefault() == true)
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