using ActiveOfficeLife.Domain;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class UserRepository : _Repository<User>, IUserRepository
    {
        public UserRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return _context.Users
                .Where(u => u.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }

        public Task<User?> GetByTokenAsync(string token)
        {
            return _context.Users
                .Where(u => u.Token == token)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            return await _context.Users.Where(x => x.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUserPassAsync(string userName, string password)
        {
            var hasPass = DomainHelper.HashPassword(password);
            return await _context.Users.Where(u => u.Username.ToLower() == userName.ToLower() && u.PasswordHash == hasPass).FirstOrDefaultAsync();
        }
    }
}
