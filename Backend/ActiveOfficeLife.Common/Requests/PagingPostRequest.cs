using ActiveOfficeLife.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Requests
{
    public class PagingPostRequest : PagingRequest
    {
        public PostStatus? Status { get; set; } = null;
    }
}
