using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models
{
    public partial class Oy
    {
        [Key]
        [Column("OyID")] // Veritabanında adı OyID
        public int OyId { get; set; }

        [Required]
        [Column("UserID")] // KRİTİK DÜZELTME: Veritabanında adı UserID, kodda KullaniciId
        public string KullaniciId { get; set; } = null!;

        [Column("AnketID")] // Veritabanında adı AnketID
        public int AnketId { get; set; }

        [Column("SecenekID")] // Veritabanında adı SecenekID
        public int SecenekId { get; set; }

        [ForeignKey("AnketId")]
        public virtual Anket Anket { get; set; } = null!;

        [ForeignKey("SecenekId")]
        public virtual AnketSecenegi AnketSecenegi { get; set; } = null!;
    }
}