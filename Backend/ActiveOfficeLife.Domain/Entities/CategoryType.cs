using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class CategoryType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }

}
