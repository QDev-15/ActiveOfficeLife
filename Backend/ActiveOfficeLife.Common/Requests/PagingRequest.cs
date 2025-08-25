using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Requests
{
    public class PagingRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortField { get; set; } = "createdAt";
        public string SortDirection { get; set; } = "desc";
        public string SearchText
        {
            get => search ?? keySearch ?? keyWord ?? string.Empty;
            set => search = value;
        }
        public int DefaultIfNull { get; set; } = 1000;
        public string? search { set; get; } = null;
        public string? keySearch { set; get; } = null;
        public string? keyWord { set; get; } = null;

        public DateTime? StartDate { set; get; } = null;

        public DateTime? EndDate { set; get; } = null;
    }
}
