using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Media
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        public MediaType Type { get; set; }

        public int ProjectId { get; set; }

        public string Description { get; set; }

        public MediaStatus Status { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }
    }
}
