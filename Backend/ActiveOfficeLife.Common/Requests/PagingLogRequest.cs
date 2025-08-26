using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Requests
{
    public class PagingLogRequest : PagingRequest
    {
        public DateTime? StartDate { set; get; } = null;

        public DateTime? EndDate { set; get; } = null;
    }
}
