using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IUserRepository : _IRepository<User>
    {
        Task<User?> GetByUserPassAsync(string userName, string password);
        Task<User?> GetByUserNameAsync(string username);
        Task<User?> GetByTokenAsync(string token);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

    }
}
