using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string RefreshTokenExpiryTime { get; set; }

        public string? Token { get; set; }         // AccessToken hoặc RefreshToken
        public string? AvatarUrl { get; set; }     // Đường dẫn avatar

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation: nhiều-nhiều với Role
        public ICollection<Role> Roles { get; set; } = new List<Role>();

        // Navigation: User viết nhiều bài Post
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        // Navigation: User có nhiều comment
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }


}
