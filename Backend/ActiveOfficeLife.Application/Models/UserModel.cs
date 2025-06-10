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

        public string? Token { get; set; }         // AccessToken hoặc RefreshToken
        public string? RefreshTokenExpiryTime { get; set; }         // AccessToken hoặc RefreshToken

        public string? AvatarUrl { get; set; } 
        public List<string> Roles { set; get; }
    }
}
