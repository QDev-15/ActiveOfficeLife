using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Visitor
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    }

}
