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
    public class AffiliateRepository : _Repository<Affiliate>, IAffiliateRepository
    {
        public AffiliateRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }
        public async Task<(int Count, IEnumerable<Affiliate> Items)> GetByPagingAsync(CustomSearch request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "shopName", "ShopName" },
                { "shopAvatar", "ShopAvatar" },
                { "isActive", "IsActive" }

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Affiliate> query = _context.Affiliates;


            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => EF.Functions.Like(c.Name.ToLower(), search)
                    || EF.Functions.Like(c.ShopAvatar, search)
                    || EF.Functions.Like(c.ShopName, search)
                    || EF.Functions.Like(c.Description, search));
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
