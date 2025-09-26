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
    public class TagRepository : _Repository<Tag>, ITagRepository
    {
        public TagRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.Include(x => x.SeoMetadata).ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            var tag = await _context.Tags
                .Include(x => x.SeoMetadata)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
            {
                return null;
            }
            return tag;
        }

        public async Task<(IEnumerable<Tag> Items, int Count)> GetAllWithPaging(PagingTagRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "slug", "Slug" }

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Tag> query = _context.Tags.AsQueryable();


            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => EF.Functions.Like(c.Name.ToLower(), search)
                    || EF.Functions.Like(c.Slug.ToLower(), search)
               );
            }

            

            int count = await query.CountAsync();
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
                            .Include(x => x.SeoMetadata)
                            .ToListAsync();
            return (items, count);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            var tag = await _context.Tags.Where(t => t.Name == name.ToLower()).FirstOrDefaultAsync();
            if (tag == null)
            {
                return null;
            }
            return tag;
        }

        public async Task<List<Tag>> GetTagsByIds(List<Guid> ids)
        {
            ids = ids ?? [];
            var tagStringIds = string.Join(',', ids.Select(x => x.ToString()).ToList());
            var tags = await _context.Tags
                .Where(t => tagStringIds.Contains(t.Id.ToString()))
                .Include(x => x.SeoMetadata)
                .ToListAsync();
            return tags;
        }
    }
}
