using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IAdRepository : _IRepository<Ad>
    {
        Task<(List<Ad> Items, int Count)> GetAllWithPaging(PagingAdRequest request);
    }
}
