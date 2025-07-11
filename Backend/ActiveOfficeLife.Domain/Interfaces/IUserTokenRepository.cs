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
        Task<UserToken?> GetByUserIdAsync(Guid userId, string ipAddress);
        Task<ICollection<UserToken>> GetAllByUserIdAsync(Guid userId);
        Task<UserToken?> GetByAccessTokenAsync(string accessToken);
        Task<UserToken?> GetByRefreshTokenAsync(string refreshToken);

    }
}
