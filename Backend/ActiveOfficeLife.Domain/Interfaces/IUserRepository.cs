using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string guidId);
        Task<User> GetByUserPassAsync(string userName, string password);
    }
}
