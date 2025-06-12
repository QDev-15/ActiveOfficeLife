using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly _IUnitOfWork _unitOfWork;
       
        public CategoryService(ICategoryRepository categoryRepository, _IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryModel> CreateCategoryAsync(CategoryModel category)
        {
            try
            {
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category), "Category cannot be null");
                }
                // Map CategoryModel to domain entity if necessary
                var categoryEntity = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description,
                    ParentId = category.ParentId,
                };
                if (category.SeoMetadata != null) { 
                    categoryEntity.SeoMetadataId = Guid.NewGuid(); // Assuming you want to create a new SEO metadata entry
                    categoryEntity.SeoMetadata = new SeoMetadata
                    {
                        Id = categoryEntity.SeoMetadataId ?? Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null, // Set to null initially, can be updated later
                        MetaTitle = category.SeoMetadata.MetaTitle,
                        MetaDescription = category.SeoMetadata.MetaDescription,
                        MetaKeywords = category.SeoMetadata.MetaKeywords
                        // Map other properties of SeoMetadata if necessary
                    };
                }
                await _categoryRepository.AddAsync(categoryEntity);
                // Commit the changes to the database
                await _unitOfWork.SaveChangeAsync();
                // Map back to CategoryModel if necessary
                return categoryEntity.ReturnModel(); // Assuming you have an extension method to convert Category to CategoryModel  
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, "Error creating category: {CategoryName}", category?.Name ?? "Unknown");
                // Handle exceptions as needed, e.g., log them
                throw new ApplicationException("An error occurred while creating the category", ex);
            }
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Category ID cannot be empty", nameof(id));
                }
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                category.IsDeleted = true;
                _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangeAsync();
                return true; // Return true if deletion was successful
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, "Error deleting category with ID: {CategoryId}", id);
                throw new ApplicationException("An error occurred while deleting the category", ex);
            }
        }
        public Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryModel> GetCategoryByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryModel> UpdateCategoryAsync(CategoryModel category)
        {
            throw new NotImplementedException();
        }
    }
}
