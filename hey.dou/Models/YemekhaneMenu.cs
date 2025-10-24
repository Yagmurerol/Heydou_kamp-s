using System.ComponentModel.DataAnnotations;

namespace HeyDOU.KampusApp.Models // Namespace'inizi kontrol edin
{
    public class YemekhaneMenu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tarih alaný zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Menü Tarihi")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Menü içeriði zorunludur.")]
        [StringLength(1000)]
        [Display(Name = "Menü Ýçeriði")]
        public string MenuIcerigi { get; set; }

        [StringLength(250)]
        [Display(Name = "Notlar")]
        public string Notlar { get; set; }
    }
}