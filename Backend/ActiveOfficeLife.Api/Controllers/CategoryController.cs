using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ActiveOfficeLife.Api.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly string _controllerName = "Category";
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTypeService _categoryTypeService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        public CategoryController(ICategoryService categoryService,  CustomMemoryCache cache, AppConfigService appConfigService, ICategoryTypeService categoryTypeService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _appConfigService = appConfigService;
            _controllerName = this.GetType().Name;
            _categoryTypeService = categoryTypeService;
        }

        // get all with paging using GET method and query parameters sortField = 'name', sortDirection = 'asc', pageIndex = 1, pageSize = 10
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategoriesPaging([FromQuery] PagingCategoryRequest request)
        {
            try
            {
                string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{request.SortField}-{request.SortDirection}-{request.PageIndex}-{request.PageSize}-{request.SearchText}";
                var cachedResult = _cache.Get<(List<CategoryModel> Categories, int count)>(cacheKey);
                if (cachedResult.Categories != null && cachedResult.Categories.Any())
                {
                    return Ok(new ResultSuccess(new
                    {
                        Items = cachedResult.Categories,
                        TotalCount = cachedResult.count,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize,
                    }));
                }
                var result = await _categoryService.GetAllCategoriesPagingAsync(request);
                // Cache the result
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                return Ok(new ResultSuccess(new {
                    Items = result.Categories,
                    TotalCount = result.count,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                }));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching paginated categories: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve paginated categories.", "400"));
            }
        }
        [AllowAnonymous]
        [HttpGet("gettype-id")]
        public async Task<IActionResult> GetCategoryTypeById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category type ID.", "400"));
            }
            try
            {
                string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{id}";
                var cachedCategoryType = _cache.Get<CategoryTypeModel>(cacheKey);
                if (cachedCategoryType != null)
                {
                    return Ok(new ResultSuccess(cachedCategoryType));
                }
                var categoryType = await _categoryTypeService.GetById(id);
                if (categoryType != null)
                {
                    // Set cache for the category type
                    _cache.Set(cacheKey, categoryType, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                    return Ok(new ResultSuccess(categoryType));
                }
                return NotFound(new ResultError("Category type not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching category type: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve category type.", "400"));
            }
        }
        [AllowAnonymous]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category ID.", "400"));
            }
            try
            {
                // Check cache first
                string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{id}";
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
                return NotFound(new ResultError("Category not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching category: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve category.", "400"));
            }
        }
        [AllowAnonymous]
        [HttpGet("types")]
        public async Task<IActionResult> GetCategoryTypes()
        {
            string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-getcategory-type";
            try
            {
                var cacheCategoryTypes = _cache.Get<List<CategoryTypeModel>>(cacheKey);
                if (cacheCategoryTypes != null && cacheCategoryTypes.Any())
                {
                    return Ok(new ResultSuccess(cacheCategoryTypes));
                }
                var categoryTypes = await _categoryTypeService.GetAll();
                if (categoryTypes != null)
                {
                    // set cache for category types
                    _cache.Set(cacheKey, categoryTypes.ToList(), TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                    return Ok(new ResultSuccess(categoryTypes));
                }
                return NotFound(new ResultError("Category types not found.", "404"));
            } catch(Exception ex)
            {
                AOLLogger.Error(ex);
                return BadRequest(new ResultError(ex.Message));
            }
        }
        [HttpPost("create-type")]
        public async Task<IActionResult> CreateCategoryType([FromBody] CategoryTypeModel categoryType)
        {
            if (categoryType == null)
            {
                return BadRequest(new ResultError("Invalid category type data.", "400"));
            }
            try
            {
                var createdCategoryType = await _categoryTypeService.Add(categoryType);
                // Clear cache after creating a new category type
                _cache.RemoveByPattern(_controllerName);
                return Ok(new ResultSuccess(createdCategoryType));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating category type: {ex.Message}", ex);
                return BadRequest(new ResultError(ex.Message));
            }
        }

        // create category using POST method and usint CategoryModel as request body from category service
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel category)
        {
            if (category == null)
            {
                return BadRequest(new ResultError("Invalid category data.", "400"));
            }
            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                // Clear cache after creating a new category
                _cache.RemoveByPattern(_controllerName);
                return Ok(new ResultSuccess(createdCategory));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating category: {ex.Message}", ex);
                return BadRequest(new ResultError(ex.Message));

            }
        }

        [HttpDelete("delete-type")]
        public async Task<IActionResult> DeleteCategoryType(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category type ID.", "400"));
            }
            try
            {
                var result = await _categoryTypeService.Delete(id);
                if (result)
                {
                    // Clear cache after deleting a category type
                    _cache.RemoveByPattern(_controllerName);
                    return Ok(new ResultSuccess("Category type deleted successfully."));
                }
                return NotFound(new ResultError("Category type not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting category type: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to delete category type.", "400"));
            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category ID.", "400"));
            }
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    // Clear cache after deleting a category
                    _cache.RemoveByPattern(_controllerName);
                    return Ok(new ResultSuccess("Category deleted successfully."));
                }
                return NotFound(new ResultError("Category not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting category: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to delete category.", "400"));
            }
        }

        [HttpPut("update-type")]
        public async Task<IActionResult> UpdateCategoryType([FromBody] CategoryTypeModel categoryType)
        {
            if (categoryType == null || categoryType.Id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category type data.", "400"));
            }
            try
            {
                var updatedCategoryType = await _categoryTypeService.Update(categoryType);
                // Clear cache after updating a category type
                _cache.RemoveByPattern(_controllerName);
                return Ok(new ResultSuccess(updatedCategoryType));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating category type: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to update category type.", "400"));
            }
        }
        // update category using PUT method and usint CategoryModel as request body from category service
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel category)
        {
            // Nếu parse sai kiểu, ASP.NET Core sẽ để giá trị default (null hoặc 0)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultError("Invalid model state.", "400"));

            }
            if (category == null || category.Id == Guid.Empty)
            {       
                return BadRequest(new ResultError("Invalid category data.", "400"));
            }
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
                // Clear cache after updating a category
                _cache.RemoveByPattern(_controllerName);
                return Ok(new ResultSuccess(updatedCategory));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating category: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to update category.", "400"                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   ));
            }
        }
    }
}
