using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using Microsoft.EntityFrameworkCore; // Include işlemi için şart
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

			// 1. Veritabanında kodu arıyoruz ve Kat tablosuyla birleştiriyoruz.
			// NOT: '_context.Mekanlars' kullanıldı. Eğer hata verirse sondaki 's' harfini silip '_context.Mekanlar' yapın.
			// Scaffold işlemi veritabanı tablosunu genelde çoğul (Mekanlars) alır.
			var mekan = _context.Mekanlars
								.Include(m => m.Kat)
								.FirstOrDefault(x => x.MekanKodu == kod);

			if (mekan == null)
			{
				return NotFound(new { mesaj = "Bu koda sahip bir yer bulunamadı." });
			}

			// 2. Bulunan veriyi sade bir pakete koyup gönderiyoruz.
			// Artık koordinat hesaplaması yok, sadece resim dönüyor.
			var sonuc = new MekanSonucModel
			{
				MekanKodu = mekan.MekanKodu,
				Aciklama = mekan.Aciklama,
				KatAdi = mekan.Kat.KatAdi,
				KrokiResimAdi = mekan.Kat.KrokiResimAdi
			};

			return Ok(sonuc);
		}
	}
}