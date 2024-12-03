using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int Price { get; set; }

        public PaymentType PaymentType { get; set; }

        public int CustomerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DesignerId { get; set; }

        public bool IsDelete { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }
    }
}
