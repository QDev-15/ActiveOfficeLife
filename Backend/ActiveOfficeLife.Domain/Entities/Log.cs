using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Log
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; } = LogLevel.Information;         // Ví dụ: Info, Warning, Error, Critical
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }            // Nếu có lỗi exception
        public string? Source { get; set; }                // Nguồn lỗi, ví dụ: Controller, Service
        public string? UserId { get; set; }                // Ai gây ra lỗi (nếu có)
        public string? IpAddress { get; set; }             // IP client (nếu cần)
        public string? RequestPath { get; set; }           // URL đang request (nếu từ Web)
    }
}
