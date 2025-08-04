using ActiveOfficeLife.Common.Models;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryModel>> GetAllCategoriesAsync();
        Task<(List<CategoryModel> Categories, int count)> GetAllCategoriesPagingAsync(string sortField, string sortDirection, int pageIndex, int pageSize);
        Task<CategoryModel> GetCategoryByIdAsync(Guid id);
        Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
        Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
