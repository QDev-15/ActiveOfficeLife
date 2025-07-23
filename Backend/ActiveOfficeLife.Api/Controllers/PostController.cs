using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Api.Controllers
{
    public class PostController : BaseController
    {
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
        }
        // get post by id
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid post ID." });
            }
            // Check if the post is cached
            var cacheKey = $"Post_{id}";
            if (_memoryCache.TryGetValue(cacheKey, out PostModel cachedPost))
            {
                return Ok(cachedPost);
            }
            try
            {
                var post = await _postService.GetById(id);
                if (post == null)
                {
                    return NotFound(new { message = "Post not found." });
                }
                // Cache the post for future requests
                _memoryCache.Set(cacheKey, post, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(post);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching post by ID: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new { message = "Failed to retrieve post." });
            }
        }

        // get post by slug
        [HttpGet("get-by-slug/{slug}")]
        public async Task<IActionResult> GetPostBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest(new { message = "Invalid post slug." });
            }
            // Check if the post is cached
            var cacheKey = $"Post_Slug_{slug}";
            if (_memoryCache.TryGetValue(cacheKey, out PostModel cachedPost))
            {
                return Ok(cachedPost);
            }
            try
            {
                var post = await _postService.GetByAlias(slug);
                if (post == null)
                {
                    return NotFound(new { message = "Post not found." });
                }
                // Cache the post for future requests
                _memoryCache.Set(cacheKey, post, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(post);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching post by slug: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new { message = "Failed to retrieve post." });
            }
        }
        // get post by category id
        [HttpGet("get-by-category/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid category ID." });
            }
            // Check if the posts are cached
            var cacheKey = $"Posts_Category_{categoryId}";
            if (_memoryCache.TryGetValue(cacheKey, out List<PostModel> cachedPosts))
            {
                return Ok(cachedPosts);
            }
            try
            {
                var posts = await _postService.GetByCategoryId(categoryId, null);
                if (posts == null || !posts.Any())
                {
                    return NotFound(new { message = "No posts found for this category." });
                }
                // Cache the posts for future requests
                _memoryCache.Set(cacheKey, posts, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 30 minutes
                return Ok(posts);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching posts by category: {ex.Message}", ex.Source, null, ex.StackTrace);
                return BadRequest(new { message = "Failed to retrieve posts." });
            }
        }
    }
}
