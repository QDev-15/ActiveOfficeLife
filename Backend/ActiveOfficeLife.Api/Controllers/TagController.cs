using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class TagController : BaseController
    {
        private readonly ITagService _tagService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        public TagController(ITagService tagService, CustomMemoryCache cache, AppConfigService appConfigService)
        {
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _cache = cache;
            _appConfigService = appConfigService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTags([FromQuery] PagingTagRequest request)
        {
            try
            {
                var result = await _tagService.Search(request);
                return Ok(new ResultSuccess(new
                {
                    Items = result.Items,
                    TotalCount = result.Count,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                }));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching paginated tags: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to retrieve paginated tags.", "400"));
            }
        }
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetTagById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid tag ID.", "400"));
            }
            try
            {
                // Check cache first
                string cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-{id}";
                var cachedTag = _cache.Get<TagModel>(cacheKey);
                if (cachedTag != null)
                {
                    return Ok(new ResultSuccess(cachedTag));
                }
                var tag = await _tagService.GetById(id.ToString());
                if (tag != null)
                {
                    // Set cache for the tag
                    _cache.Set(cacheKey, tag, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                    return Ok(new ResultSuccess(tag));
                }
                return NotFound(new ResultError("Tag not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching tag: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to retrieve tag.", "400"));
            }
        }
        // create tag using POST method and usint TagModel as request body from tag service
        [HttpPost("create")]
        public async Task<IActionResult> CreateTag([FromBody] TagModel tag)
        {
            
            try
            {
                var createdTag = await _tagService.Add(tag);
                // Clear cache after creating a new tag
                _cache.Clear();
                return Ok(new ResultSuccess(createdTag));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating tag: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError(ex.Message));

            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid tag ID.", "400"));
            }
            try
            {
                var result = await _tagService.Delete(id.ToString());
                if (result)
                {
                    // Clear cache after deleting a tag
                    _cache.Clear();
                    return Ok(new ResultSuccess("Tag deleted successfully."));
                }
                return NotFound(new ResultError("Tag not found.", "404"));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting tag: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to delete tag.", "400"));
            }
        }

        // update tag using PUT method and usint TagModel as request body from tag service
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTag([FromBody] TagModel tag)
        {
            // Nếu parse sai kiểu, ASP.NET Core sẽ để giá trị default (null hoặc 0)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultError("Invalid model state.", "400"));

            }
            if (tag == null || tag.Id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid tag data.", "400"));
            }
            try
            {
                var updatedTag = await _tagService.Update(tag);
                // Clear cache after updating a tag
                _cache.Clear();
                return Ok(new ResultSuccess(updatedTag));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating tag: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new ResultError("Failed to update tag.", "400"));
            }
        }
    }
}
