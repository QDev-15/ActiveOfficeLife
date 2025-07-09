using ActiveOfficeLife.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserStatus Status { get; set; } = UserStatus.Active; // Trạng thái người dùng

        public string? Token { get; set; }         // AccessToken hoặc RefreshToken
        public string? RefreshToken { get; set; }         // AccessToken hoặc RefreshToken

        public DateTime? CreatedAt { set; get; }
        public string? AvatarUrl { get; set; } 
        public List<string> Roles { set; get; }
    }
}
