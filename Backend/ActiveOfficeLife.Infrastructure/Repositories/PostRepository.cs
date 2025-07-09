using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class PostRepository : _Repository<Post>, IPostRepository
    {
        public PostRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<Post?> GetByAliasAsync(string slug)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Slug == slug);
            return post;
        }

        public async Task<List<Post>> GetByKeyAsync(string keyWord)
        {
            var posts = await _context.Posts
                .Where(p => p.Title.Contains(keyWord) || p.Content.Contains(keyWord) || p.Slug.Contains(keyWord))
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .ToListAsync();
            return posts;
        }

        public async Task<List<Post>> GetPostsByAuthorAsync(Guid authorId, int pageNumber, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.AuthorId == authorId)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return posts;
        }

        public async Task<List<Post>> GetPostsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return posts;   
        }

        public async Task<List<Post>> SearchAsync(string keyWord, int pageNumber, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.Title.Contains(keyWord) || p.Content.Contains(keyWord) || p.Slug.Contains(keyWord))
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return posts;
        }
    }
}
