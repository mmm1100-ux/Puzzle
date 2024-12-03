using System;
using System.Collections.Generic;

namespace Puzzle.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public string Phone { get; set; }

        public string Social { get; set; }

        public DateTime? Birthday { get; set; }

        public string Address { get; set; }

        public bool IsSpecial { get; set; }

        public string Description { get; set; }

        public int Credit { get; set; }

        public int Balance { get; set; }

        public bool? PushSms { get; set; }

        public DateTime? LastActivity { get; set; }

        public string City { get; set; }

        public bool DoneByAdmin { get; set; }

        public virtual ICollection<Project> Project { get; set; }

        public virtual ICollection<Rating> Rating { get; set; }

        public virtual ICollection<Presenter> Presenter { get; set; }

        public virtual ICollection<Token> Token { get; set; }

        public virtual ICollection<CustomerChat> CustomerChat { get; set; }

        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
