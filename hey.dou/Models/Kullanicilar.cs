using System.ComponentModel.DataAnnotations; // 1. BU SATIR EKLENDİ (veya eksikti)

namespace hey.dou.Models
{
    public class Kullanici
    {
        [Key] // 2. BU SATIR EKLENDİ (Bu, KullaniciId'nin anahtar olduğunu belirtir)
        public int KullaniciId { get; set; }

        public string? AdSoyad { get; set; }
        public string? Email { get; set; }
        public string? Sifre { get; set; }
        public string? Rol { get; set; }
    }
}