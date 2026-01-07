using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Models
{
    public class SubscriberModel
    {
        public Guid? Id { get; set; }
        public string? Email { get; set; } = null!;
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
