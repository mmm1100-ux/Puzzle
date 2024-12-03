using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Wage
    {
        public int Id { get; set; }
        public int Percent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DesignerId { get; set; }

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }
    }
}
