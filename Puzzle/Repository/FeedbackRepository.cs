using Microsoft.AspNetCore.Mvc;
using Puzzle.Data;
using Puzzle.Models;
using Puzzle.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Repository
{
    public class FeedbackRepository : IFeedbackFormRepository
    {
        private readonly PuzzleDbContext _db;

        public FeedbackRepository(PuzzleDbContext db)
        {
            _db = db;
        }

        public IQueryable<FeedbackForm> Query()
        {
            return _db.FeedbackForms.AsQueryable();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
