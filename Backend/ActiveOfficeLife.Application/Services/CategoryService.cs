using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly _IUnitOfWork _unitOfWork;
        private readonly CustomMemoryCache _cache;
       
        public CategoryService(ICategoryRepository categoryRepository, _IUnitOfWork unitOfWork, CustomMemoryCache cache)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork;
            _cache = cache;
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
                AOLLogger.Error(ex.Message, "Error deleting category with ID: {CategoryId}", id.ToString());
                throw new ApplicationException("An error occurred while deleting the category", ex);
            }
        }
        public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
        {
            string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}";
            var categories = _cache.Get<IEnumerable<CategoryModel>>(cacheKey);
            if (categories != null && categories.Any())
            {
                return categories;
            }
            var cats = await _categoryRepository.GetAllAsync();
            if (cats == null || !cats.Any())
            {
                AOLLogger.Error($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-GetAll Category not found");
                throw new KeyNotFoundException($"Categories not found");
            }
            categories = cats.Select(x => x.ReturnModel());
            _cache.Set<IEnumerable<CategoryModel>>(cacheKey, categories);
            return categories;
        }

        public async Task<CategoryModel> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-{id.ToString()}";
                var cacheValue = _cache.Get<CategoryModel>(cacheKey);
                if (cacheValue != null)
                {
                    return cacheValue;
                }
                var cat = await _categoryRepository.GetByIdAsync(id);
                if (cat == null)
                {
                    AOLLogger.Error($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-{MessageContext.NotFound}");
                    throw new KeyNotFoundException("Get Cagegory not found");
                }
                _cache.Set<CategoryModel>(cacheKey, cat.ReturnModel());
                return cat.ReturnModel();
            } catch (Exception ex)
            {
                AOLLogger.Error($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Error:{ex.Message}", ex.Source, ex.StackTrace);
                throw new Exception("Get category faild");
            }
        }

        public async Task<CategoryModel> UpdateCategoryAsync(CategoryModel category)
        {
            string msg = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}";
            try
            {
                var cat = await _categoryRepository.GetByIdAsync(category.Id);
                if (cat == null)
                {
                    AOLLogger.Error(msg + $"-Error category Id:{category.Id.ToString()}-not found");
                    throw new KeyNotFoundException("not found category update");
                }
                cat.Slug = category.Slug;
                cat.ParentId = category.ParentId;
                cat.Description = category.Description;
                cat.Name = category.Name;
                if (category.SeoMetadata != null) {
                    cat.SeoMetadata = new SeoMetadata()
                    {
                        Id = cat.SeoMetadataId ?? Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        MetaDescription = category.SeoMetadata.MetaDescription,
                        MetaKeywords = category.SeoMetadata.MetaKeywords,
                        MetaTitle = category.SeoMetadata.MetaTitle,
                        UpdatedAt = category.SeoMetadata.UpdatedAt,
                    };
                };
                _categoryRepository.UpdateAsync(cat);
                await _unitOfWork.SaveChangeAsync();
                return cat.ReturnModel();

            } catch(Exception ex)
            {
                AOLLogger.Error($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Error: {ex.Message}", ex.Source, "", ex.StackTrace);
                throw new Exception("update category faild");
            }
        }
    }
}
