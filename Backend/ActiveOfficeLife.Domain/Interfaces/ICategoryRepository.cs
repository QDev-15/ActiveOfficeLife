using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ICategoryRepository : _IRepository<Category>
    {
        Task<(IEnumerable<Category> Categories, int Count)> GetAllWithPaging(int pageIndex, int pageSize, string sortField, string sortDirection);
        Task<IEnumerable<Category>> GetByParrentId(Guid parentId);
    }
}
