using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using System.Linq;

namespace hey.dou.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HeydouContext _context;

        public AuthController(HeydouContext context) // Veritabanı bağlantısını sağlar
        {
            _context = context;
        }

        [HttpPost("giris")]
        public IActionResult GirisYap([FromBody] LoginModel gelenVeri) // Gelen kimlik bilgilerini doğrulayarak JSON formatında yanıt döner
        {
            var kullanici = _context.Kullanicilars.FirstOrDefault(x =>
                x.Email == gelenVeri.Email &&
                x.Sifre == gelenVeri.Sifre &&
                x.Rol == gelenVeri.Rol);

            if (kullanici == null)
            {
                return Unauthorized(new { mesaj = "Giriş başarısız! Bilgiler hatalı." });
            }

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