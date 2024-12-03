using Puzzle.Models;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int HistoryId { get; set; }
        public ProjectProperty Type { get; set; }

        [ForeignKey(nameof(HistoryId))]
        public virtual History History { get; set; }
    }
}
