using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
