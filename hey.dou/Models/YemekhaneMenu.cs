// Dosya: Models/YemekhaneMenu.cs

using System;
using System.ComponentModel.DataAnnotations;


namespace HeyDOU.KampusApp.Models
{
    public class YemekhaneMenu
    {
        public int Id { get; set; } 

        [Display(Name = "Menü Tarihi")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

        
        [Display(Name = "Menü Ýçeriði")]
        [Required(ErrorMessage = "Menü içeriði boþ býrakýlamaz.")]
        public string MenuIcerigi { get; set; } = string.Empty;

      
        [Display(Name = "Notlar")]
        public string? Notlar { get; set; }
    }
}