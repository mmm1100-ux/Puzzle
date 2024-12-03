using Models;
using Puzzle.Models;
using System;
using System.Collections.Generic;
using static Enums.Enum;

namespace ViewModels
{
    public class SearchPaymentViewModel
    {
        public int Id { get; set; }
        public PageViewModel Page { get; set; }
        public List<PaymentViewModel> Payment { get; set; }
    }

    public class PaymentViewModel
    {
        public int Id { get; set; }

        public Project Project { get; set; }

        public PaymentType PaymentType { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Price { get; set; }

        public int Balance { get; set; }

        public int Credit { get; set; }

        public User Designer { get; set; }

        public bool IsTransaction { get; set; }

        public Customer Customer { get; set; }

        public DateTime Receipt { get; set; }

        public string Description { get; set; }
    }
}
