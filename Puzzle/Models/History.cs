using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class History
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public int ProjectId { get; set; }

        public string Json { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User User { get; set; }

        public virtual ICollection<Property> Property { get; set; }
    }
}
