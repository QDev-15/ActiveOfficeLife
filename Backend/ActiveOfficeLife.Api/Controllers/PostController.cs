using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ActiveOfficeLife.Api.Controllers
{
    public class PostController : BaseController
    {
        private string serviceName = "Post";
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly CustomMemoryCache _memoryCache;
        private readonly AppConfigService _appConfigService;
        public PostController(IPostService postService, ICategoryService categoryService, ITagService tagService, CustomMemoryCache memoryCache, AppConfigService appConfigService)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _appConfigService = appConfigService;
            serviceName = this.GetType().Name;
        }
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPostPaging([FromQuery] PagingPostRequest request)
        {
            try
            {
                string cacheKey = $"{serviceName}-{MethodBase.GetCurrentMethod().Name}-{request.PageIndex}-{request.PageSize}-{UserId}-{request.SearchText}-{request.SortDirection}-{request.SortField}-{request.Status.ToString()}";
                var cachedResult = _memoryCache.Get<(List<PostModel> Items, int count)>(cacheKey);
                if (cachedResult != default)
                {
                    return Ok(new ResultSuccess(new
                    {
                        Items = cachedResult.Items,
                        TotalCount = cachedResult.count,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize,
                    }));
                }
                var result = await _postService.GetAll(request);
                if (result != default)
                {
                    _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
                }
                return Ok(new ResultSuccess(new
                {
                    Items = result.Items,
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
        // get post by id
        [AllowAnonymous]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid post ID.", "400"));
            }
            // Check if the post is cached
            var cacheKey = $"{serviceName}-Post_{id}";
            if (_memoryCache.TryGetValue(cacheKey, out PostModel cachedPost))
            {
                return Ok(new ResultSuccess(cachedPost));
            }
            try
            {
                var post = await _postService.GetById(id);
                if (post == null)
                {
                    return NotFound(new ResultError("Post not found.", "404"));
                }
                // Cache the post for future requests
                _memoryCache.Set(cacheKey, post, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(new ResultSuccess(post));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching post by ID: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve post.", "400"));
            }
        }

        // get post by slug
        [AllowAnonymous]
        [HttpGet("get-by-slug/{slug}")]
        public async Task<IActionResult> GetPostBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest(new ResultError("Invalid post slug.", "400"));
            }
            // Check if the post is cached
            var cacheKey = $"{serviceName}-Post_Slug_{slug}";
            if (_memoryCache.TryGetValue(cacheKey, out PostModel cachedPost))
            {
                return Ok(new ResultSuccess(cachedPost));
            }
            try
            {
                var post = await _postService.GetByAlias(slug);
                if (post == null)
                {
                    return NotFound(new ResultError("Post not found.", "404"));
                }
                // Cache the post for future requests
                _memoryCache.Set(cacheKey, post, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(new ResultSuccess(post));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching post by slug: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve post.", "400"));
            }
        }
        // get post by category id
        [AllowAnonymous]
        [HttpGet("get-by-category/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid category ID.", "400"));
            }
            // Check if the posts are cached
            var cacheKey = $"{serviceName}-Posts_Category_{categoryId}";
            if (_memoryCache.TryGetValue(cacheKey, out List<PostModel> cachedPosts))
            {
                return Ok(new ResultSuccess(cachedPosts));
            }
            try
            {
                var posts = await _postService.GetByCategoryId(categoryId, null);
                if (posts == null || !posts.Any())
                {
                    return NotFound(new ResultError("No posts found for this category.", "404"));
                }
                // Cache the posts for future requests
                _memoryCache.Set(cacheKey, posts, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(new ResultSuccess(posts));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching posts by category: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to retrieve posts.", "400"));
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] PostModel post)
        {
            post.AuthorId = post.AuthorId ?? UserId;
            if (post == null)
            {
                return BadRequest(new ResultError("Post data is required.", "400"));
            }
            try
            {
                var createdPost = await _postService.Create(post);
                _memoryCache.RemoveByPattern($"{serviceName}"); // Clear relevant caches
                return Ok(new ResultSuccess(createdPost));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating post: {ex.Message}", ex);
                return BadRequest(new ResultError("Failed to create post.", "400"));
            }
        }
        [HttpPatch("patch")]
        public async Task<IActionResult> Patch([FromBody] JsonElement requestPatch)
        {

            // Lấy id (case-insensitive). Khuyến nghị: đưa id lên route luôn cho gọn.
            if (!Helper.TryGetGuidCaseInsensitive(requestPatch, "id", out var id) || id == Guid.Empty)
                return BadRequest("Missing id");

            var current = await _postService.GetById(id);
            if (current == null) return NotFound();

            var opts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,         // serialize theo camelCase
                PropertyNameCaseInsensitive = true,                        // đọc không phân biệt hoa/thường
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            // Serialize entity hiện tại sang JsonObject
            var currentNode = JsonSerializer.SerializeToNode(current, opts)!.AsObject();

            // Parse payload patch sang JsonObject
            var patchNode = JsonNode.Parse(requestPatch.GetRawText())!.AsObject();

            // Không cho đổi Id (nếu có gửi kèm)
            patchNode.Remove("id");

            // Merge (đệ quy cho object con; array thì replace hoàn toàn)
            Helper.MergeJson(currentNode, patchNode);

            // Deserialize ngược về model
            var merged = currentNode.Deserialize<PostModel>(opts)!;

            // Bảo toàn các field gốc id, name, createdAt,...
            merged.Id = (Guid)current.Id;
            var updated = await _postService.Update(merged);
            _memoryCache.RemoveByPattern($"{serviceName}"); // Clear relevant caches
            return Ok(new ResultSuccess()
            {
                Data = updated
            });
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] PostModel setting)
        {
            if (setting == null || setting.Id == System.Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid setting data", "400"));
            }
            try
            {
                var updatedSetting = await _postService.Update(setting);
                _memoryCache.RemoveByPattern($"{serviceName}"); // Clear relevant caches
                return Ok(new ResultSuccess(updatedSetting));
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ResultError(ex.Message, "400"));
            }
        }
    }
}
