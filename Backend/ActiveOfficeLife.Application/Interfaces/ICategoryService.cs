using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryModel>> GetAllCategoriesAsync();
        Task<(List<CategoryModel> Categories, int count)> GetAllCategoriesPagingAsync(PagingCategoryRequest request);
        Task<CategoryModel> GetCategoryByIdAsync(Guid id);
        Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
        Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
