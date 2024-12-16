using LocalizingPkg.Shared.Shared;
using Puzzle.ViewModels.Feedbacks;
using System.Threading.Tasks;

namespace Puzzle.Services.Interfaces
{
    public interface IFeedbackFormService
    {
        Task<Result<FilterFeedbackFormViewModel>> Filter(FilterFeedbackFormViewModel model);
        Task<Result<FeedbackFormViewModel>> GetByIdAsync(int id);
        Task<Result> CreateAsync(UpsertFeedbackFormViewModel model);
        Task<Result> DeleteAsync(int id);
    }
}
