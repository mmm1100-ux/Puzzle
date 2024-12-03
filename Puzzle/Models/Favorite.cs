using Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
