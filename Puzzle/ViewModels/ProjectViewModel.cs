using MD.PersianDateTime.Standard;
using Models;
using Puzzle.Models;
using System;
using System.Collections.Generic;

namespace Puzzle.ViewModels
{
    public class ProjectViewModel
    {
        public int Page { get; set; }

        public int TotalPage { get; set; }

        public int TotalRow { get; set; }

        public int? Status { get; set; }

        public Enums.Enum.ProjectCategory? ProjectCategory { get; set; }

        public List<Project> Project { get; set; }

        public class Search
        {
            public string Title { get; set; }

            public string DesignerId { get; set; }

            public Enums.Enum.ProjectCategory? ProjectCategory { get; set; }

            public Enums.Enum.Quality? Type { get; set; }

            public int Page { get; set; }

            public string FromDate { get; set; }

            public string ToDate { get; set; }

            public int? FromPrice { get; set; }

            public int? ToPrice { get; set; }
        }
    }
}
