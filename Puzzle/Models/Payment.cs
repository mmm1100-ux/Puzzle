using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Price { get; set; }

        public PaymentType PaymentType { get; set; }

        public string DesignerId { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }
    }
}
