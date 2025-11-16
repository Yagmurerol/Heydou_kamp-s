using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models;

public partial class Oy
{
    [Key]
    [Column("OyID")]          // DB'deki kolon adı
    public int OyId { get; set; }

    [Required]
    [Column("UserID")]        // DB'deki kolon adı
    public string KullaniciId { get; set; } = null!;

    [Column("AnketID")]       // DB'deki kolon adı
    public int AnketId { get; set; }

    [Column("SecenekID")]     // DB'deki kolon adı
    public int SecenekId { get; set; }

    [ForeignKey("AnketId")]
    public virtual Anket Anket { get; set; } = null!;

    [ForeignKey("SecenekId")]
    public virtual AnketSecenegi AnketSecenegi { get; set; } = null!;
}
