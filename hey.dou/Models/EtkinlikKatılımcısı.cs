using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class EtkinlikKatılımcısı
{
    public int KatilimciId { get; set; }

    public int EventId { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
