using Microsoft.AspNetCore.Mvc;
using hey.dou.Models; // LoginModel'i bulabilmesi için
using System.Linq;    // Veritabanı sorgusu için gerekli

namespace hey.dou.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly HeydouContext _context;

		public AuthController(HeydouContext context)
		{
			_context = context;
		}

		// Giriş Yapma Adresi: POST /api/Auth/giris
		[HttpPost("giris")]
		public IActionResult GirisYap([FromBody] LoginModel gelenVeri)
		{
			// 1. Veritabanında bu kriterlere uyan bir kullanıcı var mı?
			// NOT: '_context.Kullanicilars' hata verirse sonundaki 's' harfini silip '_context.Kullanicilar' yapın.
			var kullanici = _context.Kullanicilars.FirstOrDefault(x =>
				x.Email == gelenVeri.Email &&
				x.Sifre == gelenVeri.Sifre &&
				x.Rol == gelenVeri.Rol);

			// 2. Eğer kullanıcı bulunamazsa hata döndür
			if (kullanici == null)
			{
				return Unauthorized(new { mesaj = "Giriş başarısız! Bilgiler hatalı." });
			}

			// 3. Giriş başarılıysa kullanıcı bilgilerini geri döndür
			return Ok(new
			{
				mesaj = "Giriş başarılı",
				kullaniciId = kullanici.KullaniciId,
				adSoyad = kullanici.AdSoyad,
				rol = kullanici.Rol
			});
		}
	}
}