using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Puzzle.Data;
using System;
using System.Linq;

namespace Puzzle.Helper
{
    public class TimeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var db = new PuzzleDbContext();

            var userId = context.HttpContext.User.GetUserId();

            var user = db.Users.Where(x => x.Id == userId).First();

            user.LastActivity = DateTime.Now;

            db.SaveChanges();

            if ((user.FromTime != null && DateTime.Now.TimeOfDay < user.FromTime) || (user.ToTime != null && DateTime.Now.TimeOfDay > user.ToTime))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
