using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Token
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public int CustomerId { get; set; }

        public int Code { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
