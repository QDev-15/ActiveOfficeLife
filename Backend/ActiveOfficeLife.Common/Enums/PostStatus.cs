using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Enums
{
    public enum PostStatus
    {         
        All,
        [Display(Name = "Bản nháp")]
        Draft,
        [Display(Name = "Ngưng xuất bản")]
        Paused,
        [Display(Name = "Xuất bản")]
        Published,
        [Display(Name = "Đóng")]
        Closed
    }
}
