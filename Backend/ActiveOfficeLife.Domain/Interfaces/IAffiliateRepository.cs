using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Common.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IAffiliateRepository : _IRepository<Affiliate>
    {
        Task<(int Count, IEnumerable<Affiliate> Items)> GetByPagingAsync(CustomSearch request);
    }
}
