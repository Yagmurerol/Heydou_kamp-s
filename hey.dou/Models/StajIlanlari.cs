using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class StajIlanlari
{
    public int Id { get; set; }

    public string Baslik { get; set; } = null!;

    public string Sirket { get; set; } = null!;

    public string Lokasyon { get; set; } = null!;

    public string StajTuru { get; set; } = null!;

    public string? Aciklama { get; set; }

    public DateOnly? YayinlanmaTarihi { get; set; }

    public DateOnly? SonBasvuru { get; set; }

    public bool? Aktif { get; set; }

    public string? BasvuruLinki { get; set; }
}
