using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System.Reflection;

namespace ActiveOfficeLife.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
       
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IPostRepository postRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork;
            _postRepository = postRepository;
        }
        public async Task<CategoryModel> CreateCategoryAsync(CategoryModel category)
        {
            try
            {
                if (category == null)
                {
                    throw new Exception("Category cannot be null");
                }
                if (category.Name == null || category.Name.Length < 3)
                {
                    throw new ArgumentException("Category name must be at least 3 characters long", nameof(category.Name));
                }
                var checkCategoryByName = await _categoryRepository.CheckExitsByName(category.Name, category.Id);
                if (checkCategoryByName)
                {
                    throw new Exception($"Category with name '{category.Name}' already exists");
                }
                var checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                if (checkCategoryBySlug)
                {
                    category.Slug = Helper.GenerateSlug(category.Name);
                    checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                    if (checkCategoryBySlug)
                    {
                        category.Slug = category.Slug + "-" + DateTime.UtcNow.Ticks; // Append a unique identifier to the slug
                        checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                        if (checkCategoryBySlug)
                        {
                            throw new Exception($"Category with slug for '{category.Name}' already exists");
                        }
                    }  
                }
                // Map CategoryModel to domain entity if necessary
                var categoryEntity = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description,
                    ParentId = category.ParentId,
                    CategoryTypeId = category.CategoryTypeId,
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
                await _unitOfWork.SaveChangesAsync();
                // Map back to CategoryModel if necessary
                return categoryEntity.ReturnModel(); // Assuming you have an extension method to convert Category to CategoryModel  
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message + " Error creating category: " + (category?.Name ?? "Unknown"), ex);
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
                var category = await _categoryRepository.GetById(id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                var checkUsing = await _categoryRepository.CheckUsing(id);
                if (checkUsing)
                {
                    category.IsActive = false; // If the category is being used, just deactivate it instead of deleting
                    _categoryRepository.Update(category);
                }
                else
                {
                    // If the category is not being used, perform a soft delete
                    _categoryRepository.Remove(category);
                }
                await _unitOfWork.SaveChangesAsync();
                return true; // Return true if deletion was successful
            }
            catch (Exception ex)
            {
                string messae = ex.Message + " Error deleting category with ID: " + id.ToString();
                AOLLogger.Error(messae, ex);
                throw new ApplicationException("An error occurred while deleting the category", ex);
            }
        }
        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
        {
            string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}";
            var categories = new List<CategoryModel>();

            var cats = await _categoryRepository.GetAllAsync();
            if (cats == null || !cats.Any())
            {
                string messageError = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-GetAll Category not found";
                AOLLogger.Error(new LogProperties() { Message = messageError });
                throw new KeyNotFoundException($"Categories not found");
            }
            categories = [];
            var queryCats = cats.Where(x => x.ParentId == null);

            foreach (var item in queryCats)
            {
                categories.Add(GetCategoryChild(item, cats));
            }
            return categories;
        }
        private CategoryModel GetCategoryChild(Category category, IEnumerable<Category> categoryModels)
        {
            var childs = categoryModels.Where(x => x.ParentId == category.Id);
            if (!childs.Any())
            {
                return category.ReturnModel();
            }

            var resultCat = category.ReturnModel();
            foreach (var item in childs)
            {
                resultCat.Children.Add(GetCategoryChild(item, categoryModels));
            }
            return resultCat;
            
        }

        public async Task<CategoryModel> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var cat = await _categoryRepository.GetById(id);
                if (cat == null)
                {
                    string message = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Get Category by Id:{id.ToString()} not found";
                    AOLLogger.Error(new LogProperties() { Message = message});
                    throw new KeyNotFoundException("Get Cagegory not found");
                }
                return cat.ReturnModel();
            } catch (Exception ex)
            {
                string messageError = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Get Category by Id:{id.ToString()} faild";
                AOLLogger.Error(new LogProperties() { Message = messageError });
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
                    string message = msg + $"-Get Category by Id:{category.Id.ToString()} not found";
                    AOLLogger.Error(new LogProperties() { Message = message });
                    throw new KeyNotFoundException("not found category update");
                }
                if (category.Name == null || category.Name.Length < 3)
                {
                    throw new ArgumentException("Category name must be at least 3 characters long", nameof(category.Name));
                }
                var checkCategoryByName = await _categoryRepository.CheckExitsByName(category.Name, category.Id);
                if (checkCategoryByName)
                {
                    throw new Exception($"Category with name '{category.Name}' already exists");
                }
                var checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                if (checkCategoryBySlug)
                {
                    category.Slug = Helper.GenerateSlug(category.Name);
                    checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                    if (checkCategoryBySlug)
                    {
                        category.Slug = category.Slug + "-" + DateTime.UtcNow.Ticks; // Append a unique identifier to the slug
                        checkCategoryBySlug = await _categoryRepository.CheckExitsBySlug(category.Slug, category.Id);
                        if (checkCategoryBySlug)
                        {
                            throw new Exception($"Category with slug for '{category.Name}' already exists");
                        }
                    }
                }
                cat.Slug = category.Slug;
                cat.ParentId = category.ParentId;
                cat.CategoryTypeId = category.CategoryTypeId;
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
                _categoryRepository.Update(cat);
                await _unitOfWork.SaveChangesAsync();
                return cat.ReturnModel();

            } catch(Exception ex)
            {
                string messageError = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Update Category Id:{category.Id.ToString()} faild";
                AOLLogger.Error(new LogProperties() { Message = messageError });
                throw new Exception("update category faild");
            }
        }

        public async Task<(List<CategoryModel> Categories, int count)> GetAllCategoriesPagingAsync(PagingCategoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortDirection))
                {
                    request.SortField = "Name"; // Default sort field
                    request.SortDirection = "asc"; // Default sort direction
                }
                var result = await _categoryRepository.GetAllWithPaging(request); // Get all categories
                if (result.Categories == null || !result.Categories.Any())
                {
                    result.Categories = new List<Category>();
                }
                
                return (result.Categories.Select(x => x.ReturnModel()).ToList(), result.Count);
            }
            catch (Exception ex)
            {
                string messageError = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Get categories paging failed";
                AOLLogger.Error(new LogProperties() { Message = messageError });
                throw new Exception("Get categories paging failed");
            }
        }
    }
}
