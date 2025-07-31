using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Requests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public ResetPasswordRequest() { }
        public ResetPasswordRequest(string email, string newPassword, string confirmPassword, string token)
        {
            Email = email;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
            Token = token;
        }
    }
}
