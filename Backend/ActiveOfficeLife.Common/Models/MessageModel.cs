using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Models
{
    public class MessageModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Subject { get; set; } = null!;
        public string? Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? StatusMessage { get; set; } = null!;
        public string? IpAddress { get; set; } = null!;
    }
}
