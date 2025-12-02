using ActiveOfficeLife.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICategoryTypeService
    {
        Task<CategoryTypeModel?> GetById(Guid id);
        Task<IEnumerable<CategoryTypeModel>> GetAll();
        Task<CategoryTypeModel?> Add(CategoryTypeModel model);
        Task<CategoryTypeModel?> Update(CategoryTypeModel model);
        Task<bool> Delete(Guid id);
    }
}
