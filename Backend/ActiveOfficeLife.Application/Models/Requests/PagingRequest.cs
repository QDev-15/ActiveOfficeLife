using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Requests
{
    public class PagingRequest
    {
        public int pageIndex { get; set; } = 0;
        public int pageSize { get; set; } = 10;
        public string? sortField { get; set; } = "createdAt";
        public string? sortDirection { get; set; } = "desc";
        public string? searchText { get; set; } = null;
        
    }
}
