using LocalizingPkg.Shared.Shared;
using Puzzle.Models;
using Puzzle.ViewModels.Feedbacks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Services.Interfaces
{
    public interface IFeedbackFormService
    {
        Task<Result<FilterFeedbackFormViewModel>> GetAllAsync(FilterFeedbackFormViewModel model);
        Task<Result<FeedbackFormViewModel>> GetByIdAsync(int id);
        Task<Result> CreateAsync(UpsertFeedbackFormViewModel model);
        Task<Result> DeleteAsync(int id);
    }
}
