using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using Microsoft.EntityFrameworkCore; // "Include" komutu için şart
using System.Linq;

namespace hey.dou.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KrokiController : ControllerBase
	{
		private readonly HeydouContext _context;

		public KrokiController(HeydouContext context)
		{
			_context = context;
		}

		// Arama Adresi: GET /api/Kroki/ara?kod=D-144
		[HttpGet("ara")]
		public IActionResult MekanAra(string kod)
		{
			if (string.IsNullOrEmpty(kod))
				return BadRequest("Lütfen bir mekan kodu girin.");

			// 1. Veritabanında kodu arıyoruz.
			// .Include(m => m.Kat) komutu sayesinde o sınıfın bağlı olduğu KAT bilgisini de çekiyoruz.

			// DİKKAT: Veritabanı tablonuzun adı 'Mekanlar' olduğu için burada '_context.Mekanlar' kullandım.
			// Eğer kod '_context.Mekanlar' kısmında kırmızı hata verirse, sonuna 's' ekleyip '_context.Mekanlars' yapmayı deneyin.
			var mekan = _context.Mekanlars
								.Include(m => m.Kat)
								.FirstOrDefault(x => x.MekanKodu == kod);

			if (mekan == null)
			{
				return NotFound(new { mesaj = "Bu koda sahip bir yer bulunamadı." });
			}

			// 2. Bulunan veriyi paketleyip gönderiyoruz
			var sonuc = new MekanSonucModel
			{
				MekanKodu = mekan.MekanKodu,
				Aciklama = mekan.Aciklama,
				KatAdi = mekan.Kat.KatAdi,
				KrokiResimAdi = mekan.Kat.KrokiResimAdi,
				PinX = mekan.KoordinatX,
				PinY = mekan.KoordinatY
			};

			return Ok(sonuc);
		}
	}
}