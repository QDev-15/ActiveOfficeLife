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
    public class CustomTableRepository : _Repository<CustomTable>, ICustomTableRepository
    {
        public CustomTableRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<(int Count, IEnumerable<CustomTable> Items)> GetByPaging(CustomSearch request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "isActive", "IsActive" },
                { "type", "Type" },
                { "startDate", "StartDate" },
                { "endDate", "EndDate" },

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<CustomTable> query = _context.CustomTables;


            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => EF.Functions.Like(c.Name.ToLower(), search)
                    || EF.Functions.Like(c.Description, search));
            }
            if (request.Date != null)
            {
                // compile date only from datetime all convert to utc, ignore time part
                var dateOnly = request.Date.Value.Date;
                query = query.Where(c => (c.StartDate !=null && c.StartDate.Value.Date == dateOnly) 
                                        || (c.EndDate != null && c.EndDate.Value.Date == dateOnly)
                                        || c.CreatedAt.Date == dateOnly || c.UpdatedAt.Date == dateOnly);
            }
            if (request.StartDate != null)
            {
                var startDateOnly = request.StartDate.Value.Date;
                query = query.Where(c => c.StartDate != null && c.StartDate.Value.Date >= startDateOnly);
            }
            if (request.EndDate != null)
            {
                var endDateOnly = request.EndDate.Value.Date;
                query = query.Where(c => c.EndDate != null && c.EndDate.Value.Date <= endDateOnly);
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
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return (count, items);
        }
    }
}
