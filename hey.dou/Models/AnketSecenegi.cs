using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models
{
    public partial class AnketSecenegi
    {
        [Key]
        public int SecenekId { get; set; } // Küçük 'd'

        [Required]
        public string SecenekText { get; set; } = null!;

        public int AnketId { get; set; } // Küçük 'd'

        [ForeignKey("AnketId")]
        public virtual Anket Anket { get; set; } = null!;

        public virtual ICollection<Oy> Oys { get; set; } = new HashSet<Oy>();
    }
}