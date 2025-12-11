using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Common.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ICustomTableRepository : _IRepository<CustomTable>
    {
         Task<(int Count, IEnumerable<CustomTable> Items)> GetByPaging(CustomSearch request);
    }
}
