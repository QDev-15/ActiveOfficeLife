using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class UserTokenRepository : _Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<ICollection<UserToken>> GetAllByUserIdAsync(Guid userId)
        {
            var userTokens = await _context.UserTokens
                .Where(ut => ut.UserId == userId)
                .ToListAsync();
            return userTokens;
        }

        public async Task<UserToken?> GetByAccessTokenAsync(string accessToken)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.AccessToken == accessToken); 
            return userToken;
        }

        public async Task<UserToken?> GetByRefreshTokenAsync(string refreshToken)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.RefreshToken == refreshToken);
            return userToken;
        }

        public async Task<UserToken?> GetByUserIdAsync(Guid userId, string ipAddress)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.IpAddress == ipAddress);
            return userToken;
        }
    }
}
