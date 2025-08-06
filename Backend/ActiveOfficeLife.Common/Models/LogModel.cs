using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Models
{
    public class LogModel
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Sử dụng UTC để đồng bộ hóa
        public string Level { get; set; } = "";// Ví dụ: Info, Warning, Error, Critical
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }            // Nếu có lỗi exception
        public string? Source { get; set; }                // Nguồn lỗi, ví dụ: Controller, Service
        public string? UserId { get; set; }                // Ai gây ra lỗi (nếu có)
        public string? IpAddress { get; set; }             // IP client (nếu cần)
        public string? RequestPath { get; set; }           // URL đang request (nếu từ Web)
    }
}
