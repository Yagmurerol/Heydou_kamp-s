using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hey.dou.Models
{
    public partial class Anket
    {
        [Key]
        public int AnketId { get; set; } // DÜZELTME: AnketID -> AnketId (Küçük d)

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<AnketSecenegi> AnketSecenegis { get; set; } = new HashSet<AnketSecenegi>();

        public virtual ICollection<Oy> Oys { get; set; } = new HashSet<Oy>();
    }
}