using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using System;
using System.Linq;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class SurveyController : Controller
    {
        private readonly PuzzleDbContext _db;

        public SurveyController()
        {
            _db = new PuzzleDbContext();
        }

        public IActionResult Index()
        {
            var X = _db.Survey.Where(c => c.Vote != 0).ToList();

            return View(X);
        }

        [HttpGet("/Vote/{Id}")]
        [AllowAnonymous]
        public IActionResult Vote(int Id)
        {
            var X = _db.Survey.SingleOrDefault(c => c.Code == Id);

            if (X == null || X.Vote != 0)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost("/Vote/{Id}")]
        [AllowAnonymous]
        public IActionResult Thanks(int Id, Survey S)
        {
            var X = _db.Survey.SingleOrDefault(c => c.Code == Id);

            X.Date = DateTime.Now;

            X.Description = S.Description;

            X.Vote = S.Vote;

            _db.Survey.Update(X);

            _db.SaveChanges();

            return View();
        }
    }
}