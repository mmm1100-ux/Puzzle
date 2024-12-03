using Models;
using Puzzle.Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class DesignerProjectViewModel
    {
        public List<Details> Project { get; set; }

        public int TotalRow { get; set; }

        public int PageNumber { get; set; }

        public class Details
        {
            public Project Project { get; set; }
            public int Balance { get; set; }
            public int Credit { get; set; }
        }
    }
}