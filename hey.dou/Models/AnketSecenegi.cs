using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class AnketSecenegi
{
    public int SecenekId { get; set; }

    public string SecenekText { get; set; } = null!;

    public int AnketId { get; set; }

    public string? CreatorKullaniciId { get; set; }

    public string? KisaAciklama { get; set; }

    public string? Konum { get; set; }

    public string? KulupAdi { get; set; }

    public string? KulupBaskaniAdi { get; set; }

    public DateTime? TarihSaat { get; set; }

    public virtual Anket Anket { get; set; } = null!;

    public virtual ICollection<Oy> Oys { get; set; } = new List<Oy>();
}
