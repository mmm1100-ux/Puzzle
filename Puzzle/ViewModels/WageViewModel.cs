using Models;
using Puzzle.Models;
using System.Collections.Generic;

namespace Puzzle.ViewModels
{
    public class WageViewModel
    {
        public double Wage { get; set; }

        public List<Deposit> Deposit { get; set; }
    }
}
