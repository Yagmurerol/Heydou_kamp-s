using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Katlar
{
    public int KatId { get; set; }

    // DÜZELTME: = null!; eklendi
    public string KatAdi { get; set; } = null!;

    // DÜZELTME: = null!; eklendi
    public string KrokiResimAdi { get; set; } = null!;

    public virtual ICollection<Mekanlar> Mekanlars { get; set; } = new List<Mekanlar>();
}