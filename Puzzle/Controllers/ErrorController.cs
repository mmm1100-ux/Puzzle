using Microsoft.AspNetCore.Mvc;
using Models;
using Puzzle.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    public class ErrorController : Controller
    {
        [HttpPost("error/add")]
        public IActionResult Add(string message)
        {
            using var db = new PuzzleDbContext();

            db.Error.Add(new Error { DateTime = DateTime.Now, StackTrace = "App!", Message = message });

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/error/add-by-app")]
        public IActionResult AddError([FromBody] Error model)
        {
            using var db = new PuzzleDbContext();

            model.DateTime = DateTime.Now;

            db.Error.Add(model);

            db.SaveChanges();

            return Ok();
        }
    }
}
