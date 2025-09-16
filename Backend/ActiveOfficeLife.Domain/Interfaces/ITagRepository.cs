using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ITagRepository : _IRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<(IEnumerable<Tag> Items, int Count)> GetAllWithPaging(PagingTagRequest request);
    }
}
