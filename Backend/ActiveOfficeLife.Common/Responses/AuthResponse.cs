
namespace ActiveOfficeLife.Common.Responses
{
    public class AuthResponse
    {
        public string UserId { get; set; }
        public string SettingId { get; set; }
        public string Email { set; get; }
        public string Status { set; get; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
    }
}
