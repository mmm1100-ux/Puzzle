using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class Presenter
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ProjectId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }
    }
}
