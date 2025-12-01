using ActiveOfficeLife.Common.Requests;
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
        Task<Category> GetDefaultCategoryAsync();
        Task<(IEnumerable<Category> Categories, int Count)> GetAllWithPaging(PagingCategoryRequest request);
        Task<IEnumerable<Category>> GetByParrentId(Guid parentId);
        Task<IEnumerable<Category>> GetByCategoryType(Guid typeId);
        Task<Category?> GetById(Guid id);
        Task<bool> CheckExitsByName(string name, Guid id);
        Task<bool> CheckExitsBySlug(string slug, Guid id);
        Task<bool> CheckUsing(Guid id);
    }
}
