using Puzzle.Shared;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.ViewModels.Feedbacks
{
    public class FeedbackFormViewModel : BaseViewModel<int>
    {
        public string DesignerId { get; set; }

        public string DesignerName { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

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

    }
}
