using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ActiveOfficeLife.Api.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        public CategoryController(ICategoryService categoryService, CustomMemoryCache cache, AppConfigService appConfigService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _appConfigService = appConfigService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            // cache key for categories
            string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}";

            try
            {
                var chedCategories = _cache.Get<List<CategoryModel>>(cacheKey);
                if (chedCategories != null)
                {
                    return Ok(new ResultSuccess(chedCategories));
                }
                var categories = await _categoryService.GetAllCategoriesAsync();
                if (categories == null || !categories.Any())
                {
                    return NotFound(new ResultError("No categories found." ));
                }
                _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Set cache for 30 minutes
                return Ok(new ResultSuccess(categories));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching categories: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to retrieve categories."));
            }
        }

        // get all with paging using GET method and query parameters sortField = 'name', sortDirection = 'asc', pageIndex = 1, pageSize = 10
        [HttpGet("all-paging")]
        public async Task<IActionResult> GetAllCategoriesPaging([FromQuery] PagingRequest request)
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesPagingAsync(request);
                return Ok(new ResultSuccess(new {
                    Items = result.Categories,
                    TotalCount = result.count,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                }));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching paginated categories: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to retrieve paginated categories."));
            }
        }
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category ID."));
            }
            try
            {
                // Check cache first
                string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-{id}";
                var cachedCategory = _cache.Get<CategoryModel>(cacheKey);
                if (cachedCategory != null)
                {
                    return Ok(new ResultSuccess(cachedCategory));
                }
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category != null)
                {
                    // Set cache for the category
                    _cache.Set(cacheKey, category, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                    return Ok(new ResultSuccess(category));
                }
                return NotFound(new ResultError("Category not found."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching category: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to retrieve category."));
            }
        }
        // create category using POST method and usint CategoryModel as request body from category service
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel category)
        {
            if (category == null)
            {
                return BadRequest(new ResultError("Invalid category data."));
            }
            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                // Clear cache after creating a new category
                _cache.Clear();
                return Ok(new ResultSuccess(createdCategory));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating category: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to create category."));

            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category ID."));
            }
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    // Clear cache after deleting a category
                    _cache.Clear();
                    return Ok(new ResultSuccess("Category deleted successfully."));
                }
                return NotFound(new ResultError("Category not found."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting category: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to delete category."));
            }
        }

        // update category using PUT method and usint CategoryModel as request body from category service
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromForm] CategoryModel category)
        {
            if (category == null || category.Id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category data."));
            }
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
                // Clear cache after updating a category
                _cache.Clear();
                return Ok(new ResultSuccess(updatedCategory));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating category: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to update category."));
            }
        }
    }
}
