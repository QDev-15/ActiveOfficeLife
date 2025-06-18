using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;         // Ví dụ: Admin, Author, Viewer
        public string Description { get; set; } = null!;  // Mô tả quyền
        public List<UserModel> Users { get; set; } = new List<UserModel>();
    }
}
