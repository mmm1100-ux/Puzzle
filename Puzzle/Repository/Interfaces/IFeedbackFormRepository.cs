using Puzzle.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Repository.Interfaces
{
    public interface IFeedbackFormRepository
    {
        IQueryable<FeedbackForm> Query();

        //Add
        Task AddAsync(FeedbackForm feedbackForm);

        //Update
        void UpdateAsync(FeedbackForm feedbackForm);

        //SaveChanges
        Task SaveChangesAsync();
    }
}
