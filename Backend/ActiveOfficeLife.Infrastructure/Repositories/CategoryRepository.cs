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
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(x => x.IsDeleted == false).Include(x => x.SeoMetadata).ToListAsync();
        }
        public async Task<(IEnumerable<Category> Categories, int Count)> GetAllWithPaging(int pageIndex, int pageSize, string sortField, string sortDirection)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "slug", "Slug" },
                { "isActive", "IsActive" }
            };
            if (!allowedFields.TryGetValue(sortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
            }
            IQueryable<Category> query = _context.Categories.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(actualSortField))
            {
                if (sortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, actualSortField));
                }
                else
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, actualSortField));
                }
            }

            int count = await query.CountAsync();
            var categories = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Include(x => x.SeoMetadata).ToListAsync();
            return (categories, count);
        }

        public async Task<Category?> GetByIdAsync(Guid guidId)
        {
            return await _context.Categories.Include(x => x.SeoMetadata).FirstOrDefaultAsync(x => x.Id == guidId);
        }

        public async Task<IEnumerable<Category>> GetByParrentId(Guid parentId)
        {
            return await _context.Categories.Where(x => x.ParentId == parentId).Include(x => x.SeoMetadata).ToListAsync();
        }
    }
}
