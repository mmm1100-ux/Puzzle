using Puzzle.Data;
using System;
using System.Linq;

namespace Repository
{
    public class RatingRepository
    {
        public int GetTotalProject(int CustomerId, string designerId = null)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => designerId == null || x.DesignerId == designerId)
                .Where(c => c.CustomerId == CustomerId)
                .Count();
        }

        public int GetSumLastMonth(int CustomerId)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.CustomerId == CustomerId)
                .Where(c => c.Receipt >= DateTime.Now.AddMonths(-1))
                .Count();
        }

        public DateTime? GetLastTime(int CustomerId, string designerId = null)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => designerId == null || x.DesignerId == designerId)
                .Where(c => c.CustomerId == CustomerId)
                .OrderByDescending(c => c.Receipt)
                .FirstOrDefault()
                ?.Receipt;
        }

        public DateTime? GetTimeOfMembership(int CustomerId)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.CustomerId == CustomerId)
                .OrderBy(c => c.Receipt)
                .FirstOrDefault()
                ?.Receipt;
        }
    }
}
