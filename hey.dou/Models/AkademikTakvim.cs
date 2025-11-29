using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class AkademikTakvim
{
    public int EventId { get; set; }

    public DateOnly BaslangicTarihi { get; set; }

    public DateOnly BitisTarihi { get; set; }

    public string Aciklama { get; set; } = null!;

    public string? Kategori { get; set; }

    public string? RenkKodu { get; set; }
}
