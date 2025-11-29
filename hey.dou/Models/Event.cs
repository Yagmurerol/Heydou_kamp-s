using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    public int? AnketId { get; set; }

    public bool? AnketSonucuSecildi { get; set; }

    public bool? KatilimOylamasiAcik { get; set; }

    public virtual Anket? Anket { get; set; }

    public virtual ICollection<EtkinlikKatılımcısı> EtkinlikKatılımcısıs { get; set; } = new List<EtkinlikKatılımcısı>();
}
