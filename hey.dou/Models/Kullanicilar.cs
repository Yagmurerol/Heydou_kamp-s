using System.ComponentModel.DataAnnotations;

namespace hey.dou.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }

        // Soru işaretleri (?) ekleyerek uyarıları susturuyoruz
        public string? AdSoyad { get; set; }
        public string? Email { get; set; }
        public string? Sifre { get; set; }
        public string? Rol { get; set; }
    }
}