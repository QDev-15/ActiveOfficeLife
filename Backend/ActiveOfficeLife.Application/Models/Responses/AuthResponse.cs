using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Responses
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserModel User { get; set; } = new UserModel();
        public string Role { get; set; }
    }
}
