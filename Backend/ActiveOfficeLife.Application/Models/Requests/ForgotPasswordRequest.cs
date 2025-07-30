using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Requests
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = default!; // Email của người dùng để đặt lại mật khẩu
        public string? PhoneNumber { get; set; } // Số điện thoại của người dùng (tùy chọn)
        public string? FullName { get; set; } // Tên đầy đủ của người dùng (tùy chọn)
        public string? Username { get; set; } // Tên đăng nhập của người dùng (tùy chọn)
    }
}
