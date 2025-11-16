using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models;

public partial class Oy
{
    [Key]
    public int OyId { get; set; }

    [Required]
    public string KullaniciId { get; set; } = null!;

    public int AnketId { get; set; }
    public int SecenekId { get; set; } // Düzeltildi

    [ForeignKey("AnketId")]
    public virtual Anket Anket { get; set; } = null!;

    // DÜZELTME: İlişkisel özelliğin doğru adı 'AnketSecenegi'dir.
    [ForeignKey("SecenekId")]
    public virtual AnketSecenegi AnketSecenegi { get; set; } = null!;
}