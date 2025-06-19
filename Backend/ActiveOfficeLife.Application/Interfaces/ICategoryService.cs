using ActiveOfficeLife.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryModel>> GetAllCategoriesAsync(bool? nocache);
        Task<CategoryModel> GetCategoryByIdAsync(Guid id);
        Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
        Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
