using Microsoft.AspNetCore.Mvc.Filters;
using Puzzle.Data;
using System;
using System.Linq;

namespace Puzzle.Helper
{
    public class UpdateLastActivityAttribute : ActionFilterAttribute
    {
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    // Do something before the action executes.
        //}

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.

            var db = new PuzzleDbContext();

            var key = context.HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token != null && key != null)
            {
                var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

                customer.LastActivity = DateTime.Now;

                db.SaveChanges();
            }
        }
    }
}
