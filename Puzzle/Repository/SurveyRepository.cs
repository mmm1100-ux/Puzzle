using Helper;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Linq;

namespace Repository
{
    public class SurveyRepository
    {
        private readonly PuzzleDbContext _db;

        public SurveyRepository()
        {
            _db = new PuzzleDbContext();
        }

        ~SurveyRepository()
        {
            _db.Dispose();
        }

        public void SendSMS()
        {
            var X = _db.Project.Where(c => c.Delivery.HasValue ? c.Delivery.Value.Date.AddDays(1) == DateTime.Today : false)
                .Include(c => c.Survey)
                .Include(c => c.Customer)
                .ToList();

            Random R = new Random();

            foreach (var item in X)
            {
                if (item.Survey == null)
                {
                    var S = new Survey()
                    {
                        Code = R.Next(),
                        Date = DateTime.Now,
                        ProjectId = item.Id
                    };

                    _db.Survey.Add(S);
                    _db.SaveChanges();

                    SMSHelper.SendByPattern(item.Customer.Mobile, item.Customer.FirstName + " " + item.Customer.LastName, "5510valuso", "www.puzzlearchitect.ir/vote/" + S.Code);
                }
            }

            var B = _db.Customer.Where(c => c.Birthday.Value.DayOfYear == DateTime.Today.DayOfYear).ToList();

            foreach (var item in B)
            {
                SMSHelper.SendByPattern(item.Mobile, item.FirstName + " " + item.LastName, "r4g12x29gq");
            }
        }
    }
}
