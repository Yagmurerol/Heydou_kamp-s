using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hey.dou.Models
{
    public partial class AnketSecenegi
    {
        [Key]
        // DÜZELTME: SecenekID -> SecenekId
        public int SecenekId { get; set; }

        [Required]
        public string SecenekText { get; set; } = null!;

        // DÜZELTME: AnketID -> AnketId
        public int AnketId { get; set; }

        // DÜZELTME: AnketID -> AnketId
        [ForeignKey("AnketId")]
        public virtual Anket Anket { get; set; } = null!;

        public virtual ICollection<Oy> Oys { get; set; } = new HashSet<Oy>();
    }
}