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
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneNumberAsync(string phone);
        Task<User?> GetByTokenAsync(string token);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        // get all users with paging
        Task<List<User>> GetAllAsync(int index, int pageSize);
        // search user by keyword and page, order by created date desc
        Task<List<User>> SearchAsync(string keyword, int index, int pageSize, bool desc);
    }
}
