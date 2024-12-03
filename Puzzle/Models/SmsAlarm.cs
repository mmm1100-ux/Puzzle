using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Models
{
    public class SmsAlarm
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CustomerId { get; set; }

        public string UserId { get; set; }

        public int TrackingId { get; set; }

        //public int Type { get; set; }
    }
}
