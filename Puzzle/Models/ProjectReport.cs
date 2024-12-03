using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class ProjectReport
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public string Description { get; set; }

        public ProjectReportReason Reason { get; set; }

        public bool ReadByAdmin { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
