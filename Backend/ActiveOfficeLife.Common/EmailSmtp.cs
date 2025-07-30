using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common
{
    public class EmailSmtp
    {
        public string SmtpHost { get; set; } = default!;
        public int SmtpPort { get; set; } = 587; // Mặc định là 587 cho TLS
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string From { get; set; } = default!; // Địa chỉ email gửi đi
        public string FromName { get; set; } = default!; // Tên hiển thị của người gửi
    }
}
