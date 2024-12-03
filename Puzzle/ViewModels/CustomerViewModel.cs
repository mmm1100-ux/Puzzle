using MD.PersianDateTime.Standard;
using Models;
using Puzzle.Models;
using Puzzle.ViewModels;
using System;
using System.Collections.Generic;
using static Enums.Enum;

namespace ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Social { get; set; }
        public PersianDateTime? Birthday { get; set; }
        public PersianDateTime? Delivery { get; set; }
        public PersianDateTime? Receipt { get; set; }
        public DateTime CT { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public UserViewModel User { get; set; }
        public ProjectCategory ProjectCategory { get; set; }
        public Quality? Type { get; set; }

        public ProjectViewModel Project { get; set; }
        public int TotalProject { get; set; }
        public int SumLastMonth { get; set; }
        public string LastTime { get; set; }
        public DateTime? TimeOfMembership { get; set; }
        public string Survey { get; set; }
        public Status Status { get; set; }
        public int DeliveryTime { get; set; }
        public string Presenter { get; set; }
        public string DesignerId { get; set; }
        public OrderType OrderType { get; set; }
        public int TotalPrice { get; set; }
        public List<Payment> Payment { get; set; }
        public int Approximate { get; set; }
        public bool Print { get; set; }
        public bool PrintDelivery { get; set; }
        public bool IsSpecial { get; set; }
        public bool Whatsapp { get; set; }
        public bool Telegram { get; set; }
        public bool Other { get; set; }
        public bool Screenshot { get; set; }
        public bool Follow { get; set; }
        public bool Notification { get; set; }
        public bool Ready { get; set; }
        public int Credit { get; set; }
        public int Balance { get; set; }
        public int DesignerPercent { get; set; }
        public string CustomerDescription { get; set; }
        //public List<Media> Media { get; set; }
        public List<CustomerChat> CustomerChat { get; set; }

        public bool DoneByAdmin { get; set; }

        public bool Flash { get; set; }

        public bool Eitaa { get; set; }

        public bool App { get; set; }

        public bool Cancel { get; set; }

        public bool Archive { get; set; }

        public List<Conversation> Conversation { get; set; }

        public List<Project> Child { get; set; }

        public List<Transaction> Transaction { get; set; }
        public DateTime? LastActivity { get; set; }

        public int? PrintPrice { get; set; }
    }
}
