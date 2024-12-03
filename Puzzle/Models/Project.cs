using Puzzle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static Enums.Enum;

namespace Puzzle.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int CustomerId { get; set; }

        public int? DiscountId { get; set; }

        public DateTime? Delivery { get; set; }

        public DateTime Receipt { get; set; }

        public DateTime CT { get; set; }

        public string CB { get; set; }

        public string DesignerId { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public ProjectCategory ProjectCategory { get; set; }

        public OrderType OrderType { get; set; }

        public int TotalPrice { get; set; }

        public int Approximate { get; set; }

        public int TotalPayment { get; set; }

        public double Wage { get; set; }

        public bool Print { get; set; }

        public int? PrintPrice { get; set; }

        public bool PrintDelivery { get; set; }

        public bool Whatsapp { get; set; }

        public bool Telegram { get; set; }

        public bool Other { get; set; }

        public bool Screenshot { get; set; }

        public bool Notification { get; set; }

        public bool Ready { get; set; }

        public bool Follow { get; set; }

        public bool IsDelete { get; set; }

        public int DesignerPercent { get; set; }

        public bool Soroush { get; set; }

        public bool Eitaa { get; set; }

        public bool Rubika { get; set; }

        public bool App { get; set; }

        public bool Flash { get; set; }

        public bool MediaDone { get; set; }

        public bool Cancel { get; set; }

        public Quality? Type { get; set; }

        public int? ProjectId { get; set; }

        public bool DoneByDesigner { get; set; }

        public DateTime? AssignedToDesignerAt { get; set; }

        public bool Archive { get; set; }

        [ForeignKey(nameof(DesignerId))]
        public virtual User Designer { get; set; }

        [ForeignKey(nameof(CB))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(DiscountId))]
        public virtual Discount Discount { get; set; }

        public virtual Survey Survey { get; set; }

        public virtual Presenter Presenter { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }

        public virtual ICollection<History> History { get; set; }

        //public virtual ICollection<Media> Media { get; set; }

        public virtual ICollection<Conversation> Conversation { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Parent { get; set; }

        public virtual ICollection<Project> Child { get; set; }

        public virtual ICollection<Favorite> Favorite { get; set; }

        public virtual ICollection<ProjectReport> ProjectReport { get; set; }

        public virtual ICollection<ProjectAccounting> ProjectAccounting { get; set; }
    }
}
