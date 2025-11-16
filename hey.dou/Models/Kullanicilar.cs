using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string AdSoyad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public string Rol { get; set; } = null!;
}
