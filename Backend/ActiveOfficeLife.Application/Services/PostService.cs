using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System.Reflection;

namespace ActiveOfficeLife.Application.Services
{
    public class PostService : IPostService
    {
        private string serviceName = "";
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            serviceName = this.GetType().Name;
        }

        public async Task<PostModel> Create(PostModel post)
        {
            string msgHdr = serviceName + "-" + nameof(Create);
            try { 
                if (post == null)
                {
                    AOLLogger.Error($"{msgHdr}: Post is null.");
                    throw new ArgumentNullException(nameof(post), "Post cannot be null.");
                }
                var newPost = new Domain.Entities.Post
                {
                    Id = Guid.NewGuid(),
                    Title = post.Title,
                    Slug = Helper.GenerateSlug(post.Title),
                    Content = post.Content,
                    Summary = post.Summary,
                    AuthorId = post.AuthorId ?? Guid.Empty, // Assuming AuthorId is required
                    CategoryId = post.CategoryId ?? Guid.Empty, // Assuming CategoryId is required
                    Status = post.Status ?? PostStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _postRepository.AddAsync(newPost);
                await _unitOfWork.SaveChangesAsync();
                return newPost.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"{serviceName}-{nameof(Create)}: Error creating post. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                    AOLLogger.Error($"{msgHdr}: Post with id {id} not found.");
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
                AOLLogger.Error($"{msgHdr}: Error deleting post with id {id}. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                    AOLLogger.Error($"{msgHdr}: Post with slug {slug} not found.");
                    return null; // Post not found
                }
                return post.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"{serviceName}-{nameof(GetByAlias)}: Error retrieving post by alias. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                    AOLLogger.Error($"{msgHdr}: Post with id {id} not found.");
                    return null; // Post not found
                }
                return post.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"{serviceName}-{nameof(GetById)}: Error retrieving post by id. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                AOLLogger.Error($"{serviceName}-{nameof(GetByKey)}: Error retrieving posts by keyword. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                AOLLogger.Error($"{serviceName}-{nameof(Search)}: Error searching posts. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                existingPost.Summary = post.Summary;
                existingPost.Status = post.Status ?? PostStatus.Draft;
                existingPost.UpdatedAt = DateTime.UtcNow;
                _postRepository.Update(existingPost);
                await _unitOfWork.SaveChangesAsync();
                
                return existingPost.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"{serviceName}-{nameof(Update)}: Error updating post. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                AOLLogger.Error($"{serviceName}-{nameof(GetByCategoryId)}: Error retrieving posts by category. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
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
                AOLLogger.Error($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-Error: {ex.Message}", ex.Source, "", ex.StackTrace);
                throw new Exception("Get categories paging failed");
            }
        }
    }
}
