using MD.PersianDateTime.Standard;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Enums.Enum;

namespace ViewModels
{
    public class ProjectAddViewModel
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
        public string Address { get; set; }
        public string Description { get; set; }
        public ProjectCategory ProjectCategory { get; set; }
        public Status Status { get; set; }
        public int DeliveryTime { get; set; }
        public string Presenter { get; set; }
        public string DesignerId { get; set; }
        public OrderType OrderType { get; set; }
        public string TotalPrice { get; set; }
        public PaymentType?[] PaymentType { get; set; }
        public string[] PaymentDate { get; set; }
        public string[] PaymentDescription { get; set; }
        public string[] PaymentPrice { get; set; }
        public string[] PaymentDesignerId { get; set; }

        public string Approximate { get; set; }

        public bool Print { get; set; }

        public int? PrintPrice { get; set; }

        public bool Whatsapp { get; set; }

        public bool Telegram { get; set; }

        public bool Flash { get; set; }

        public bool App { get; set; }

        public bool Other { get; set; }

        [Range(0, 100)]
        public int DesignerPercent { get; set; }

        public Quality Type { get; set; }

        public bool PrintDelivery { get; set; }

        public bool Soroush { get; set; }

        public bool Eitaa { get; set; }

        public bool Rubika { get; set; }

        public bool Screenshot { get; set; }

        public bool Notification { get; set; }

        public bool Ready { get; set; }

        public bool Follow { get; set; }

        public int ProjectId { get; set; }
    }
}
