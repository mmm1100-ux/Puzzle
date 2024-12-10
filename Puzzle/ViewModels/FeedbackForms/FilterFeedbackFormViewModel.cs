using LocalizingPkg.Shared;

namespace Puzzle.ViewModels.Feedbacks
{
    public class FilterFeedbackFormViewModel : BasePaging<FeedbackFormViewModel>
    {
        public string? DesignerId { get; set; }

        public int? CustomerId { get; set; }

        public int? Vote { get; set; }
    }
}
