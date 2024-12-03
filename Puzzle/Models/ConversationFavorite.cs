using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class ConversationFavorite
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ConversationId))]
        public Conversation Conversation { get; set; }
    }
}
