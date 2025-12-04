using ActiveOfficeLife.Common.Requests;
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

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .Include(p => p.SeoMetadata)
                .FirstOrDefaultAsync(p => p.Id == id);
            return post;
        }
        public async Task<(List<Post> Items, int Count)> GetAllWithPaging(PagingPostRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "title", "Title" },
                { "slug", "Slug" },
                { "status", "Status" },
                { "category", "Category" },

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Title"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Post> query = _context.Posts.AsQueryable();

            if (request.Status != null && request.Status != Common.Enums.PostStatus.All)
            {
                query = query.Where(p => p.Status == request.Status);
            }

            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => (c.Category != null && EF.Functions.Like(c.Category.Name.ToLower(), search))
                    || EF.Functions.Like(c.Title.ToLower(), search) 
                    || EF.Functions.Like(c.Slug.ToLower(), search)
                    || EF.Functions.Like(c.Content.ToLower(), search)
                    || EF.Functions.Like(c.Summary!.ToLower(), search)
               );
            }

            if (actualSortField.ToLower() == "category")
            {
                //sort category first with parent null, and group by parent category
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(c => c.CategoryId == null ? 0 : 1).ThenBy(c => c.Category.Name).ThenBy(c => c.Title);
                }
                else
                {
                    query = query.OrderByDescending(c => c.CategoryId == null ? 0 : 1).ThenBy(c => c.Category.Name).ThenBy(c => c.Title);
                }
                
            }
            else if (!string.IsNullOrEmpty(actualSortField))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, actualSortField));
                }
                else
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, actualSortField));
                }
            }

            int count = await query.CountAsync();
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
                            .Include(x => x.Category)
                            .Include(x => x.Author)
                            .Include(x => x.Tags)
                            .Include(x => x.SeoMetadata)
                            .ToListAsync();
            return (items, count);
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

        public async Task<(List<Post> Items, int Count)> GetList(PagingPostRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "title", "Title" },
                { "slug", "Slug" },
                { "status", "Status" },
                { "category", "Category" },

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Title"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Post> query = _context.Posts.AsQueryable();

            if (request.Status != null && request.Status != Common.Enums.PostStatus.All)
            {
                query = query.Where(p => p.Status == request.Status);
            }

            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => (c.Category != null && EF.Functions.Like(c.Category.Name.ToLower(), search))
                    || EF.Functions.Like(c.Title.ToLower(), search)
                    || EF.Functions.Like(c.Slug.ToLower(), search)
                    || EF.Functions.Like(c.Content.ToLower(), search)
                    || EF.Functions.Like(c.Summary!.ToLower(), search)
               );
            }

            if (actualSortField.ToLower() == "category")
            {
                //sort category first with parent null, and group by parent category
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(c => c.CategoryId == null ? 0 : 1).ThenBy(c => c.Category.Name).ThenBy(c => c.Title);
                }
                else
                {
                    query = query.OrderByDescending(c => c.CategoryId == null ? 0 : 1).ThenBy(c => c.Category.Name).ThenBy(c => c.Title);
                }

            }
            else if (!string.IsNullOrEmpty(actualSortField))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, actualSortField));
                }
                else
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, actualSortField));
                }
            }

            int count = await query.CountAsync();
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
                            .Include(x => x.Category)
                            .ToListAsync();
            return (items, count);
        }
    }
}
