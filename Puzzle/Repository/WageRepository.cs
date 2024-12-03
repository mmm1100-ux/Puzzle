using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Linq;

namespace Repository
{
    public class WageRepository
    {
        public Wage GetByDesignerId(string designerId)
        {
            using var db = new PuzzleDbContext();

            var result = db.Wage
                .Where(c => c.DesignerId == designerId)
                .OrderByDescending(c => c.CreatedAt)
                .First();

            return result;
        }

        public void Add(string designerId, int percent)
        {
            using var db = new PuzzleDbContext();

            var wage = db.Wage
                .Where(c => c.DesignerId == designerId)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefault();

            if (wage == null || wage.Percent != percent)
            {
                db.Wage.Add(new Wage
                {
                    CreatedAt = DateTime.Now,
                    DesignerId = designerId,
                    Percent = percent
                });

                db.SaveChanges();
            }
        }
    }
}
