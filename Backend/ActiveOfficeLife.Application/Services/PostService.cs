using ActiveOfficeLife.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveOfficeLife.Application.Common;

namespace ActiveOfficeLife.Application.Services
{
    public class PostService : IPostService
    {
        private string serviceName = "";
        private readonly IPostRepository _postRepository;
        private readonly _IUnitOfWork _unitOfWork;
        public PostService(IPostRepository postRepository, _IUnitOfWork unitOfWork)
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
                _postRepository.RemoveAsync(post);
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

        public Task<PostModel> GetByAlias(string slug)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostModel>> GetByKey(string keyWord)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostModel>> Search(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> Update(PostModel post)
        {
            throw new NotImplementedException();
        }
    }
}
