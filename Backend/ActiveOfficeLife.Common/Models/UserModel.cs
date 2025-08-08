using ActiveOfficeLife.Common.Enums;

namespace ActiveOfficeLife.Common.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; } // Số điện thoại người dùng
        public string? FullName { get; set; }   // Tên đầy đủ của người dùng
        public string Status { get; set; } = UserStatus.Active.ToString(); // Trạng thái người dùng

        public string? Token { get; set; }         // AccessToken hoặc RefreshToken
        public string? RefreshToken { get; set; }         // AccessToken hoặc RefreshToken

        public DateTime? CreatedAt { set; get; }
        public string? AvatarUrl { get; set; } 
        public List<string> Roles { set; get; }
    }
}
