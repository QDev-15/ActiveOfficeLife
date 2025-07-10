using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IUserTokenRepository : _IRepository<UserToken>
    {
        Task<UserToken?> GetByUserIdAsync(string userId, string ipAddress);
        Task<ICollection<UserToken>> GetAllByUserIdAsync(string userId);
        Task<UserToken?> GetByAccessTokenAsync(string accessToken);
        Task<UserToken?> GetByRefreshTokenAsync(string refreshToken);

    }
}
