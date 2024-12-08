using Puzzle.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puzzle.Models
{
    public class FeedbackForm : BaseEntity<int>
    {
        [Required]
        public string DesignerId { get; set; }

        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        public string? Description { get; set; }

        [Range(1, 5)]
        public int Vote { get; set; }

        [Range(1, 5)]
        public int? DesignQualityVote { get; set; }

        [Range(1, 5)]
        public int? ComplianceOfTheOrderWithTheDesignVote { get; set; }

        [Range(1, 5)]
        public int? AppropriateApproachOfTheDesignerVote { get; set; }

        [Range(1, 5)]
        public int? PriceVote { get; set; }

        [Range(1, 5)]
        public int? AppropriateTreatmentOfManagementAndOfficeWorkersVote { get; set; }

        #region Relations

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        #endregion
    }
}
