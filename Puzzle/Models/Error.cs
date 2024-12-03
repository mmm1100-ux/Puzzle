using System;

namespace Models
{
    public class Error
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public bool Read { get; set; }
    }
}
