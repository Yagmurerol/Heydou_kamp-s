using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models;

public partial class Event
{
    [Key]
    // DÜZELTME: EventID -> EventId
    public int EventId { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }

    // DÜZELTME: AnketID -> AnketId
    public int? AnketId { get; set; }
    [ForeignKey("AnketId")]
    public virtual Anket? Anket { get; set; }

    // === 6. FONKSİYON İÇİN EKLENEN ALANLAR ===
    public bool AnketSonucuSecildi { get; set; } = false;
    public bool KatilimOylamasiAcik { get; set; } = false;

    public virtual ICollection<Katilim> Katilimlar { get; set; } = new List<Katilim>();
}