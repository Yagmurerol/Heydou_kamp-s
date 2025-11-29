using hey.dou.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hey.dou.Controllers
{
	[Authorize]
	public class EtkinlikController : Controller
	{
		private readonly HeydouContext _context;

		public EtkinlikController(HeydouContext context)
		{
			_context = context;
		}

		// --- Etkinlik Listesi ---
		public async Task<IActionResult> Index()
		{
			// DÜZELTME 1: 'bool?' hatasını önlemek için .GetValueOrDefault() == true kullanıyoruz.
			var secilenEtkinlikler = await _context.Events
												   .Where(e => e.AnketSonucuSecildi.GetValueOrDefault() == true)
												   .ToListAsync();

			return View(secilenEtkinlikler);
		}

		// --- Katılımı Kaydetme ---
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> KatilimiKaydet(int etkinlikId, bool katiliyor)
		{
			// 1. Kullanıcı ID'sini String olarak alıyoruz
			var kullaniciIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(kullaniciIdString))
			{
				return Unauthorized();
			}

			// DÜZELTME 2: 'int' ile 'string' karşılaştırma hatasını çözüyoruz.
			// String olan ID'yi güvenli bir şekilde sayıya (int) çeviriyoruz.
			if (!int.TryParse(kullaniciIdString, out int kullaniciId))
			{
				return BadRequest("Hatalı Kullanıcı ID formatı.");
			}

			// 3. Veritabanı sorgusu (Artık int ile int karşılaştırılıyor, hata vermez)
			// NOT: '_context.Katilimlars' yerine '_context.Katilimlar' olabilir.
			// Hata alırsanız sondaki 's' harfini silin.
			var katilimKaydi = await _context.Katilimlars
											 .FirstOrDefaultAsync(k => k.EtkinlikId == etkinlikId && k.KullaniciId == kullaniciId);

			if (katilimKaydi == null)
			{
				// Yeni kayıt oluştur
				katilimKaydi = new Katilim
				{
					EtkinlikId = etkinlikId,
					KullaniciId = kullaniciId,
					KatilimDurumu = katiliyor,
					// DÜZELTME 3: 'Required member' hatasını önlemek için boş nesne ataması
					Etkinlik = null!
				};
				_context.Katilimlars.Add(katilimKaydi);
			}
			else
			{
				// Var olan kaydı güncelle
				katilimKaydi.KatilimDurumu = katiliyor;
				_context.Katilimlars.Update(katilimKaydi);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Katilimcilar), new { id = etkinlikId });
		}

		// --- Katılımcıları Görüntüleme ---
		public async Task<IActionResult> Katilimcilar(int id)
		{
			var etkinlik = await _context.Events.FindAsync(id);

			// DÜZELTME 4: Nullable bool hatasını önlemek için GetValueOrDefault() kullanıyoruz
			if (etkinlik == null || etkinlik.KatilimOylamasiAcik.GetValueOrDefault() == false)
			{
				return NotFound();
			}

			var katilimcilar = await _context.Katilimlars
											 .Where(k => k.EtkinlikId == id && k.KatilimDurumu.GetValueOrDefault() == true)
											 .ToListAsync();

			ViewBag.EtkinlikAd = etkinlik.Title;

			return View(katilimcilar);
		}
	}
}