using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ICategoryTypeRepository : _IRepository<CategoryType>
    {
        // add, edit, delete inherited from _IRepository
        Task<IEnumerable<CategoryType>> GetAll();
        Task<CategoryType?> GetById(Guid id);
    }
}
