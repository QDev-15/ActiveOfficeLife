using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common
{
    public class LogProperties
    {
        public string? Source { get; set; }
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }  
        public string? RequestPath { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
        public string? StackTrace { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? TimestampUtc { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
