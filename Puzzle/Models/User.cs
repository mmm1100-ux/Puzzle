using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Puzzle.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Birthday { get; set; }

        public string Image { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Priority { get; set; }

        public TimeSpan? FromTime { get; set; }

        public TimeSpan? ToTime { get; set; }

        public DateTime? LastActivity { get; set; }

        public virtual ICollection<Wage> Wage { get; set; }
    }
}
