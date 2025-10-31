using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // Proje adınız 'hey.dou' ve modelleriniz 'Models' klasöründe
using System.Threading.Tasks;
using System.Collections.Generic;

namespace hey.dou.Controllers
{
	// API adresi: /api/AkademikTakvim
	[Route("api/[controller]")]
	[ApiController]
	public class AkademikTakvimController : ControllerBase
	{
		// ----- ÖNEMLİ KONTROL 1 -----
		// Veritabanı köprünüzü (Context) tutmak için bir değişken.
		// Adının 'HeydouContext' olduğundan emin olun.
		// (Models klasörünüzdeki ...Context.cs dosyasının adını kontrol edin)
		private readonly HeydouContext _context;

		// Dependency Injection ile veritabanı context'ini alıyoruz
		// (Program.cs'de AddDbContext sayesinde bu otomatik çalışır)
		public AkademikTakvimController(HeydouContext context)
		{
			_context = context;
		}

		// --- İŞTE BU SİZİN ENDPOINT'İNİZ ---
		// Frontend ekibi bu metoda GET isteği atacak
		// Adresi: GET /api/AkademikTakvim
		[HttpGet]
		public async Task<IActionResult> GetButunTakvimVerileri()
		{
			try
			{
				// ----- ÖNEMLİ KONTROL 2 -----
				// 'AkademikTakvim' tablonuza erişiyoruz.
				// 'Context' dosyanızın içinde DbSet<AkademikTakvim> özelliğinin adının
				// 'AkademikTakvims' olduğundan emin olun (genellikle 's' takısı alır).
				var takvimVerileri = await _context.AkademikTakvims.ToListAsync();

				// Verileri JSON formatında ve 200 OK (Başarılı) koduyla döndür.
				return Ok(takvimVerileri);
			}
			catch (System.Exception ex)
			{
				// Bir hata olursa 500 koduyla hatayı döner
				return StatusCode(500, $"Sunucu Hatası: {ex.Message}");
			}
		}
	}
}
