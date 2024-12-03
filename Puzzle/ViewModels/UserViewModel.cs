using MD.PersianDateTime.Standard;
using Models;
using Puzzle.Models;
using Puzzle.ViewModels;
using System;
using System.Collections.Generic;

namespace ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public int Percent { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public PersianDateTime? Birthday { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TodayCount { get; set; }
        public double TodayPrice { get; set; }
        public int Priority { get; set; }

        public List<Deposit> Deposit { get; set; }
        public ProjectViewModel Project { get; set; }
        public List<Payment> Payment { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ProjectTotalRow { get; set; }

        public int PaymentTotalRow { get; set; }

        public double TotalWage { get; set; }

        public double Wage { get; set; }

        public TimeSpan? FromTime { get; set; }

        public TimeSpan? ToTime { get; set; }
    }
}
