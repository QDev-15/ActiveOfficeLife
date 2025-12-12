using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System.Net.WebSockets;
using System.Reflection;

namespace ActiveOfficeLife.Application.Services
{
    public class PostService : IPostService
    {
        private string serviceName = "";
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork, 
            ICategoryRepository categoryRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            serviceName = this.GetType().Name;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        public async Task<PostModel> Create(PostModel post)
        {
            string msgHdr = serviceName + "-" + nameof(Create);
            try { 
                if (post == null)
                {
                    string message = msgHdr + ": Post cannot be null.";
                    AOLLogger.Error(new LogProperties() { Message = message });
                    throw new ArgumentNullException(nameof(post), "Post cannot be null.");
                }
                if (string.IsNullOrWhiteSpace(post.Title))
                {
                    // set default title if not provided
                    post.Title = "Active office life - Post " + DateTime.UtcNow.ToShortTimeString();
                }
                // check and sett default category if not provided
                if (post.CategoryId == null || post.CategoryId == Guid.Empty)
                {
                    var defaultCategory = await _categoryRepository.GetDefaultCategoryAsync();
                    // set default category id (you may want to fetch this from a config or constant)
                    post.CategoryId = defaultCategory.Id; // Replace with actual default category ID
                }
                var newPost = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = post.Title,
                    Slug = Helper.GenerateSlug(post.Title),
                    Content = post.Content ?? string.Empty,
                    Summary = post.Summary,
                    AuthorId = post.AuthorId ?? Guid.Empty, // Assuming AuthorId is required
                    CategoryId = post.CategoryId ?? Guid.Empty, // Assuming CategoryId is required
                    IsCenterHighlight = post.IsCenterHighlight,
                    IsFeaturedHome = post.IsFeaturedHome,
                    IsHot = post.IsHot,
                    DisplayOrder = post.DisplayOrder ?? 0,
                    Status = string.IsNullOrEmpty(post.Status) ? PostStatus.Draft : Enum.Parse<PostStatus>(post.Status),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SeoMetadata = new SeoMetadata
                    {
                        Id = Guid.NewGuid(),
                        MetaTitle = post.SeoMetadata?.MetaTitle,
                        MetaDescription = post.SeoMetadata?.MetaDescription,
                        MetaKeywords = post.SeoMetadata?.MetaKeywords,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                await _postRepository.AddAsync(newPost);
                await _unitOfWork.SaveChangesAsync();
                return newPost.ReturnModel();
            }
            catch (Exception ex)
            {
                string message = msgHdr + $"{serviceName}-{nameof(Create)}: Error creating post. Exception message: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            string msgHdr = serviceName + "-" + nameof(Delete);
            try { 
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    string message = msgHdr + $": Post with id {id} not found.";
                    AOLLogger.Error(new LogProperties() { Message = message });
                    return false; // Post not found
                }
                _postRepository.Remove(post);
                await _unitOfWork.SaveChangesAsync();
                // Logic to delete the post by id
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                string message = msgHdr + $": Exception occurred while deleting post with id {id}. Exception message: {ex.Message}";
                AOLLogger.Error(message, ex);
                return false;
            }
        }

        public async Task<PostModel> GetByAlias(string slug)
        {
            string msgHdr = serviceName + "-" + nameof(GetByAlias);
            try { 
                var post = await _postRepository.GetByAliasAsync(slug);
                if (post == null)
                {
                    string message = msgHdr + $": Post with slug {slug} not found.";
                    AOLLogger.Error(new LogProperties() { Message = message });
                    return null; // Post not found
                }
                return post.ReturnModel();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(GetByAlias)}: Error retrieving post by alias. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<PostModel> GetById(Guid id)
        {
            string msgHdr = serviceName + "-" + nameof(GetById);
            try {
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    string message = msgHdr + $": Post with id {id} not found.";
                    AOLLogger.Error(new LogProperties() { Message = message });
                    return null; // Post not found
                }
                return post.ReturnModel();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(GetById)}: Error retrieving post by id. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PostModel>> GetByKey(string keyWord)
        {
            string msgHdr = serviceName + "-" + nameof(GetByKey);
            try {
                var posts = await _postRepository.GetByKeyAsync(keyWord);
                if (posts == null || !posts.Any())
                {
                    AOLLogger.Error($"{msgHdr}: No posts found for keyword {keyWord}.");
                    return null; // No posts found
                }
                return posts.Select(p => p.ReturnModel()).ToList();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(GetByKey)}: Error retrieving posts by keyword. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PostModel>> Search(PagingPostRequest request)
        {
            string msgHdr = serviceName + "-" + nameof(Search);
            try {
                if (request == null)
                {
                    AOLLogger.Error($"{msgHdr}: PagingRequest is null.");
                    throw new ArgumentNullException(nameof(request), "PagingRequest cannot be null.");
                }
                var posts = await _postRepository.SearchAsync(request.SearchText, request.PageIndex, request.PageSize);
                if (posts == null || !posts.Any())
                {
                    AOLLogger.Error($"{msgHdr}: No posts found for keyword {request.SearchText}.");
                    return new List<PostModel>(); // No posts found
                }
                return posts.Select(p => p.ReturnModel()).ToList();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(Search)}: Error searching posts. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<PostModel> Update(PostModel post)
        {
            string msgHdr = serviceName + "-" + nameof(Update);
            try {
                if (post == null || post.Id == null)
                {
                    AOLLogger.Error($"{msgHdr}: Post is null.");
                    throw new ArgumentNullException(nameof(post), "Post cannot be null.");
                }
                var existingPost = await _postRepository.GetByIdAsync(post.Id ?? Guid.NewGuid());
                if (existingPost == null)
                {
                    AOLLogger.Error($"{msgHdr}: Post with id {post.Id} not found.");
                    return null; // Post not found
                }

                if (post.TagIds!.Any())
                {
                    var tags = await _tagRepository.GetTagsByIds(post.TagIds);
                    existingPost.Tags = tags.ToList();
                }
                existingPost.Title = post.Title?? "AOL title " + Helper.GenerateRandomString(4);
                existingPost.Slug = post.Slug ?? Helper.GenerateSlug(existingPost.Title);
                existingPost.Content = post.Content ?? string.Empty;
                existingPost.Summary = post.Summary;
                existingPost.IsCenterHighlight = post.IsCenterHighlight;
                existingPost.IsFeaturedHome = post.IsFeaturedHome;
                existingPost.IsHot = post.IsHot;
                existingPost.DisplayOrder = post.DisplayOrder ?? 0;
                existingPost.CategoryId = post.CategoryId ?? existingPost.CategoryId;
                existingPost.Status = string.IsNullOrEmpty(post.Status)
                    ? PostStatus.Draft 
                    : (PostStatus)Enum.Parse(typeof(PostStatus), post.Status.Trim(), ignoreCase: true);
                existingPost.UpdatedAt = DateTime.UtcNow;
                if (post.SeoMetadata != null)
                {
                    if (existingPost.SeoMetadata == null)
                    {
                        existingPost.SeoMetadata = new SeoMetadata()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                    }
                    existingPost.SeoMetadata.MetaTitle = post.SeoMetadata.MetaTitle;
                    existingPost.SeoMetadata.MetaDescription = post.SeoMetadata.MetaDescription;
                    existingPost.SeoMetadata.MetaKeywords = post.SeoMetadata.MetaKeywords;
                    existingPost.SeoMetadata.UpdatedAt = DateTime.UtcNow;
                }
                _postRepository.Update(existingPost);
                await _unitOfWork.SaveChangesAsync();
                
                return existingPost.ReturnModel();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(Update)}: Error updating post. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PostModel>> GetByCategoryId(Guid categoryId, PagingPostRequest? request)
        {
            try
            {
                string msgHdr = serviceName + "-" + nameof(GetByCategoryId);
                if (request == null)
                {
                    // set default values for paging request
                    request = new PagingPostRequest();
                    request.PageSize = request.DefaultIfNull;
                }
                var posts = await _postRepository.GetPostsByCategoryAsync(categoryId, request.PageIndex, request.PageSize);
                return posts.Select(p => p.ReturnModel()).ToList();
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(GetByCategoryId)}: Error retrieving posts by category. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<PostModel> Items, int count)> GetAll(PagingPostRequest? request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortDirection))
                {
                    request.SortField = "Title"; // Default sort field
                    request.SortDirection = "asc"; // Default sort direction
                }
                var result = await _postRepository.GetAllWithPaging(request); // Get all categories
                if (result.Items == null || !result.Items.Any())
                {
                    result.Items = new List<Post>();
                }

                return (result.Items.Select(x => x.ReturnModel()).ToList(), result.Count);
            }
            catch (Exception ex)
            {
                string message = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Error: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception("Get categories paging failed");
            }
        }

        public async Task<(List<PostModel> Items, int count)> GetList(PagingPostRequest? request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortDirection))
                {
                    request.SortField = "Title"; // Default sort field
                    request.SortDirection = "asc"; // Default sort direction
                }
                var result = await _postRepository.GetList(request); // Get all categories
                if (result.Items == null || !result.Items.Any())
                {
                    result.Items = new List<Post>();
                }

                return (result.Items.Select(x => x.ReturnModel()).ToList(), result.Count);
            }
            catch (Exception ex)
            {
                string message = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Error: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception("Get categories paging failed");
            }
        }

        public async Task<HotNewsResponse> GetHotNews()
        {
            var hostNewsResponse = new HotNewsResponse();
            var hotNews = await _postRepository.GetHotNewsAsync();
            var featuredHome = await _postRepository.GetFeaturedHomeAsync();
            var centerHighlight = await _postRepository.GetCenterHighlightAsync();

            hostNewsResponse.CenterHighlight = centerHighlight.Select(x => x.ReturnModel()).ToList();
            hostNewsResponse.FeaturedHome = featuredHome.Select(x => x.ReturnModel()).ToList();
            hostNewsResponse.HotNews = hotNews.Select(x => x.ReturnModel()).ToList();

            return hostNewsResponse;
        }
    }
}
