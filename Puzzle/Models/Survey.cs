using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Survey
    {
        public int Id { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public int Vote { get; set; }

        public int ProjectId { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }
    }
}
