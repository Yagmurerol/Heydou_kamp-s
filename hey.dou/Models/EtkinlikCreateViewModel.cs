using System;
using System.ComponentModel.DataAnnotations;

namespace hey.dou.Models
{
   
    public class EtkinlikCreateViewModel
    {
        [Required(ErrorMessage = "Kulüp Adı zorunludur.")]
        [Display(Name = "Kulüp Adı")]
        public string KulupAdi { get; set; } = null!;

        [Required(ErrorMessage = "Başkan Adı zorunludur.")]
        [Display(Name = "Kulüp Başkanı Adı")]
        public string KulupBaskaniAdi { get; set; } = null!;

        [Required(ErrorMessage = "Etkinlik Adı zorunludur.")]
        [Display(Name = "Etkinlik Adı")]
        public string EtkinlikAdi { get; set; } = null!;

        [Required(ErrorMessage = "Konum zorunludur.")]
        public string Konum { get; set; } = null!;

        [Required(ErrorMessage = "Tarih ve Saat zorunludur.")]
        [Display(Name = "Etkinlik Tarihi")]
        public DateTime TarihSaat { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [Display(Name = "Kısa Açıklama")]
        public string KisaAciklama { get; set; } = null!;
    }
}