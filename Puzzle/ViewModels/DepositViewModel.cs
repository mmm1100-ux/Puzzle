using Models;
using Puzzle.Models;
using System.Collections.Generic;

namespace Puzzle.ViewModels
{
    public class DepositViewModel
    {
        public int Page { get; set; }

        public int TotalPage { get; set; }

        public List<Deposit> Deposit { get; set; }
    }
}
