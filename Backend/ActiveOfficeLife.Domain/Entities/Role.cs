using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;         // Ví dụ: Admin, Author, Viewer
        public string Description { get; set; } = null!;  // Mô tả quyền

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
