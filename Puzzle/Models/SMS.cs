using System;
using static Enums.Enum;

namespace Models
{
    public class SMS
    {
        public int Id { get; set; }

        public DateTime CT { get; set; }

        public string Message { get; set; }

        public Status Status { get; set; }

        public SMSCategory SMSCategory { get; set; }
    }
}
