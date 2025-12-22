using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models;

public class Katilim
{
	[Key]

	public int KatilimId { get; set; }

	public int EtkinlikId { get; set; }

	[Required]
	public int KullaniciId { get; set; }

	public bool? KatilimDurumu { get; set; }

	[ForeignKey("EtkinlikId")]
	public virtual Event Etkinlik { get; set; } = null!;
}