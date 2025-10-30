// Dosya: Models/YemekhaneMenu.cs
// 'Namespace' HATASINI GÝDERMEK ÝÇÝN DÜZELTÝLMÝÞ KOD

using System;
using System.ComponentModel.DataAnnotations;

// HATA BURADAYDI: Namespace'iniz tam olarak bu satýrdaki gibi olmalý
namespace HeyDOU.KampusApp.Models
{
    public class YemekhaneMenu
    {
        public int Id { get; set; } // Veritabaný için birincil anahtar (Primary Key)

        [Display(Name = "Menü Tarihi")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

        // 'Non-nullable' hatasý için düzeltme (Boþ olamaz)
        [Display(Name = "Menü Ýçeriði")]
        [Required(ErrorMessage = "Menü içeriði boþ býrakýlamaz.")]
        public string MenuIcerigi { get; set; } = string.Empty;

        // 'Non-nullable' hatasý için düzeltme (Boþ olabilir)
        [Display(Name = "Notlar")]
        public string? Notlar { get; set; }
    }
}