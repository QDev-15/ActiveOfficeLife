using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Affiliate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Url { get; set; }
        public string? ShopName { get; set; }
        public string? ShopId { get; set; }
        public string? ShopAvatar { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }


    }
}
