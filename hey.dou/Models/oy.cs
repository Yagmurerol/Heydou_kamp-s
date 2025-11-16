using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace hey.dou.Models;

public partial class Oy
{
    [Key]
    public int OyID { get; set; }
    [Required]
    public string UserID { get; set; } = null!;
    public int AnketID { get; set; }
    public int SecenekID { get; set; }
    [ForeignKey("AnketID")]
    public virtual Anket Anket { get; set; } = null!;
    [ForeignKey("SecenekID")]
    public virtual AnketSecenegi AnketSecenegi { get; set; } = null!;
}