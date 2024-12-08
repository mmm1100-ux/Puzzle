using Puzzle.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Repository.Interfaces
{
    public interface IFeedbackFormRepository
    {
        IQueryable<FeedbackForm> Query();

        //SaveChanges
        Task SaveChangesAsync();
    }
}
