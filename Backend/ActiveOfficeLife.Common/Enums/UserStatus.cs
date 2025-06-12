using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Enums
{
    public enum UserStatus
    {
        Active = 1,       // Người dùng đang hoạt động
        Inactive = 2,     // Người dùng không hoạt động
        Banned = 3,       // Người dùng bị cấm
        Pending = 4,      // Người dùng đang chờ xác nhận
        Deleted = 5       // Người dùng đã bị xóa
    }
}
