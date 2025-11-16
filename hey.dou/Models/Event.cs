using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace hey.dou.Models;

public partial class Event
{
    [Key]
    public int EventID { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }

    // Bu etkinlik hangi anketten geldi? (SQL'e karşılık gelir)
    public int? AnketID { get; set; }
    [ForeignKey("AnketID")]
    public virtual Anket? Anket { get; set; }

    // Not: 6. fonksiyon (Katılımcılar) şimdilik burada yok.
}