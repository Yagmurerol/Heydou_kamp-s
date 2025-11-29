using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Mekanlar
{
    public int MekanId { get; set; }

    public string MekanKodu { get; set; } = null!;

    public string? Aciklama { get; set; }

    public int KoordinatX { get; set; }

    public int KoordinatY { get; set; }

    public int KatId { get; set; }

    public virtual Katlar Kat { get; set; } = null!;
}
