using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int CustomerId { get; set; }
        public int Grade { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
