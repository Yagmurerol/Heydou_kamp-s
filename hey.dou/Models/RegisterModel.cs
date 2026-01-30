using System.ComponentModel.DataAnnotations;

namespace hey.dou.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
        public string Sifre { get; set; }

        [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor")]
        public string SifreKonfirm { get; set; }

        public string Rol { get; set; } = "Ogrenci";
    }
}
