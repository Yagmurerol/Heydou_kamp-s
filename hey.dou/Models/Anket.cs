using System;
using System.Collections.Generic;

namespace hey.dou.Models;

public partial class Anket
{
    public int AnketId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AnketSecenegi> AnketSecenegis { get; set; } = new List<AnketSecenegi>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Oy> Oys { get; set; } = new List<Oy>();
}
