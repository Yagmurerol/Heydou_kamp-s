using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models;

public class Katilim
{
    [Key]
    public int Id { get; set; }

    // Etkinlik ID'si zaten doğruydu, EtkinlikId olarak kabul ediyoruz.
    public int EtkinlikId { get; set; }

    // DÜZELTME: KullaniciId olarak kabul edildi.
    [Required]
    public required string KullaniciId { get; set; }

    public bool KatilimDurumu { get; set; }

    [ForeignKey("EtkinlikId")]
    public required virtual Event Etkinlik { get; set; }
}