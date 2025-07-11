﻿using ActiveOfficeLife.Common;
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

        public async Task<List<PostModel>> Search(PagingRequest request)
        {
            string msgHdr = serviceName + "-" + nameof(Search);
            try {
                if (request == null)
                {
                    AOLLogger.Error($"{msgHdr}: PagingRequest is null.");
                    throw new ArgumentNullException(nameof(request), "PagingRequest cannot be null.");
                }
                var posts = await _postRepository.SearchAsync(request.searchText, request.pageIndex, request.pageSize);
                if (posts == null || !posts.Any())
                {
                    AOLLogger.Error($"{msgHdr}: No posts found for keyword {request.searchText}.");
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
                _postRepository.UpdateAsync(existingPost);
                await _unitOfWork.SaveChangesAsync();
                
                return existingPost.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"{serviceName}-{nameof(Update)}: Error updating post. Exception: {ex.Message}", ex.Source, null, ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
    }
}
