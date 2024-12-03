using System;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Discount
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CT { get; set; }

        public string Code { get; set; }

        public int Price { get; set; }

        public Status Status { get; set; }

        public DiscountCategory DiscountCategory { get; set; }
    }
}
