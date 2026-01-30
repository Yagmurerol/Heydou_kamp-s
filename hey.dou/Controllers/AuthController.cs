using Microsoft.AspNetCore.Mvc;
using hey.dou.Models;
using System.Linq;
using BCrypt.Net;

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

        [HttpPost("login")]
        public ActionResult<ApiResponse<dynamic>> Login([FromBody] LoginModel gelenVeri) // Gelen kimlik bilgilerini doğrulayarak JSON formatında yanıt döner
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(false, "Giriş bilgileri geçersiz", 400));
                }

                var kullanici = _context.Kullanicilars.FirstOrDefault(x =>
                    x.Email == gelenVeri.Email &&
                    x.Rol == gelenVeri.Rol);

                if (kullanici == null || string.IsNullOrEmpty(kullanici.Sifre) || !BCrypt.Net.BCrypt.Verify(gelenVeri.Sifre, kullanici.Sifre))
                {
                    return Unauthorized(new ApiResponse(false, "Giriş başarısız! E-posta, şifre veya rol hatalı.", 401));
                }

                return Ok(new ApiResponse<dynamic>(true, "Giriş başarılı", new
                {
                    kullaniciId = kullanici.KullaniciId,
                    adSoyad = kullanici.AdSoyad,
                    email = kullanici.Email,
                    rol = kullanici.Rol
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterModel model) // Yeni kullanıcı kaydı oluşturur
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(false, "Geçersiz veriler", 400));
                }

                var kullaniciVarMi = _context.Kullanicilars.FirstOrDefault(x => x.Email == model.Email);
                if (kullaniciVarMi != null)
                {
                    return BadRequest(new ApiResponse(false, "Bu e-posta zaten kullanımda", 400));
                }

                var yeniKullanici = new Kullanicilar
                {
                    AdSoyad = model.AdSoyad,
                    Email = model.Email,
                    Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre),
                    Rol = model.Rol ?? "Ogrenci"
                };

                _context.Kullanicilars.Add(yeniKullanici);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "Kayıt başarıyla oluşturuldu", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "Hata: " + ex.Message, 500));
            }
        }
    }
}