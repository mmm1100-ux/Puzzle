using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Extensions
{
    public static class AuthenticationExtensions
    {
        public static string GetUserId(this ClaimsPrincipal Claim)
        {
            if (Claim == null)
            {
                throw new ArgumentNullException(nameof(Claim));
            }

            return Claim.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static User GetInfo(this ClaimsPrincipal Claim)
        {
            using var db = new PuzzleDbContext();

            return db.Users.Where(x => x.Id == Claim.GetUserId()).First();
        }
    }
}
