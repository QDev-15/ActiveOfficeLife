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
        public Guid? CategoryId { get; set; } = null;
        public Guid? CategoryTypeId { get; set; } = null;
        public string CategorySlug { get; set; } = null;
    }
}
