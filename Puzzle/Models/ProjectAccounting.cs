using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class ProjectAccounting
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TotalPrice { get; set; }

        public int Approximate { get; set; }

        public int TotalPayment { get; set; }

        public double Wage { get; set; }

        public int? PrintPrice { get; set; }

        public string DesignerId { get; set; }

        public int DesignerPercent { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
