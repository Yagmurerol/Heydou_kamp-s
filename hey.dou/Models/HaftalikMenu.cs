using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class HaftalikMenu
{
    public int MenuId { get; set; }

    public DateOnly Tarih { get; set; }

    public string Gun { get; set; } = null!;

    public string? Corba { get; set; }

    public string AnaYemek { get; set; } = null!;

    public string? YardimciYemek { get; set; }

    public string? TatliVeyaMeyve { get; set; }
}
