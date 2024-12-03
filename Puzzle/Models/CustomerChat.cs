using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Puzzle.Models
{
    public class CustomerChat
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Attachment { get; set; }

        public bool UnreadByAdmin { get; set; }

        public bool UnreadByCustomer { get; set; }

        public bool IsDelete { get; set; }

        public int CustomerId { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
