using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models
{
    public partial class AnketSecenegi
    {
        [Key]
        public int SecenekId { get; set; }

        [Required]
        // Bu, Oylama Ekranında görünen ana başlıktır (örn: "Hafta Sonu Yürüyüşü")
        public string SecenekText { get; set; } = null!;

        public int AnketId { get; set; }

        // Bu, kimin önerdiğini tutan alandır (Controller'da kullanıldı)
        public string? CreatorKullaniciId { get; set; }

        // === YENİ EKLENEN DETAY KOLONLARI (Formdan gelenler) ===
        public string? KulupAdi { get; set; }
        public string? KulupBaskaniAdi { get; set; }
        public string? Konum { get; set; }
        public DateTime? TarihSaat { get; set; } // Etkinlik tarihi
        public string? KisaAciklama { get; set; }
        // ========================================================

        [ForeignKey("AnketId")]
        public virtual Anket Anket { get; set; } = null!;

        public virtual ICollection<Oy> Oys { get; set; } = new HashSet<Oy>();
    }
}