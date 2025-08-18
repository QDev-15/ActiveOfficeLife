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
    public class CategoryRepository : _Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ActiveOfficeLifeDbContext context) : base(context)
        {

        }

        public async Task<bool> CheckExitsByName(string name, Guid id)
        {
            bool exists = await _context.Categories.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != id && x.IsDeleted == false);
            return exists;
        }

        public async Task<bool> CheckExitsBySlug(string slug, Guid id)
        {
            bool exists = await _context.Categories.AnyAsync(x => x.Slug.ToLower() == slug.ToLower() && x.Id != id && x.IsDeleted == false);
            return exists;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(x => x.IsDeleted == false).Include(x => x.SeoMetadata).ToListAsync();
        }
        public async Task<(IEnumerable<Category> Categories, int Count)> GetAllWithPaging(PagingRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "slug", "Slug" },
                { "isActive", "IsActive" }
            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Category> query = _context.Categories.Where(x => x.IsDeleted == false);
            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => EF.Functions.Like(c.Name.ToLower(), search) || EF.Functions.Like(c.Slug.ToLower(), search));
            }

            if (!string.IsNullOrEmpty(actualSortField))
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
            var categories = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Include(x => x.Parent).Include(x => x.SeoMetadata).ToListAsync();
            return (categories, count);
        }

        public async Task<Category?> GetById(Guid id)
        {
            return await _context.Categories.Include(x => x.SeoMetadata).Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Category>> GetByParrentId(Guid parentId)
        {
            return await _context.Categories.Where(x => x.ParentId == parentId).Include(x => x.SeoMetadata).ToListAsync();
        }
    }
}
