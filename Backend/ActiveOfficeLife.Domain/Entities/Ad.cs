using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Ad
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!; // Banner, Text,...
        public string? ImageUrl { get; set; }
        public string? Link { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; } = true; // Active or Inactive
    }

}
