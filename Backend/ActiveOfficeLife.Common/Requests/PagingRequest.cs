using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Requests
{
    public class PagingRequest
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortField { get; set; } = "createdAt";
        public string SortDirection { get; set; } = "desc";
        public string? SearchText { get; set; } = null;
        public int DefaultIfNull { get; set; } = 1000;
    }
}
