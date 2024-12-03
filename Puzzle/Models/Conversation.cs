using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string UserId { get; set; }

        //public int? CustomerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ConversationId { get; set; }

        public string Message { get; set; }

        public string Attachment { get; set; }

        public string FileName { get; set; }

        public int? FileSize { get; set; }

        public Enums.Enum.Role Role { get; set; }

        public bool Accepted { get; set; }

        public MediaType? Type { get; set; }

        public bool UnreadByAdmin { get; set; }

        public bool UnreadByDesigner { get; set; }

        public bool UnreadByCustomer { get; set; }

        public bool IsDelete { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(ConversationId))]
        public virtual Conversation Parent { get; set; }

        public ICollection<ConversationFavorite> ConversationFavorite { get; set; }
    }
}
