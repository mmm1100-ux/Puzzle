using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Deposit
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DesignerId { get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }
    }
}
