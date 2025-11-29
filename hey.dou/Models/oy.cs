using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Oy
{
    public int OyId { get; set; }

    public int AnketId { get; set; }

    public int SecenekId { get; set; }

    public string KullaniciId { get; set; } = null!;

    public virtual Anket Anket { get; set; } = null!;

    public virtual AnketSecenegi Secenek { get; set; } = null!;
}
