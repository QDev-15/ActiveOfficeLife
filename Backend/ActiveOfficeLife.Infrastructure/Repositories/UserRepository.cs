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

        public async Task<List<User>> GetAllAsync(int index, int pageSize)
        {
            var users = await _context.Users
                .Include(x => x.Roles)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return users;
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return _context.Users
                .Where(u => u.RefreshToken == refreshToken)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();
        }

        public Task<User?> GetByTokenAsync(string token)
        {
            return _context.Users
                .Where(u => u.Token == token)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            return await _context.Users.Where(x => x.Username.ToLower() == username.ToLower())
                .Include(x => x.Roles).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUserPassAsync(string userName, string password)
        {
            var hasPass = DomainHelper.HashPassword(password);
            return await _context.Users.Where(u => u.Username.ToLower() == userName.ToLower() && u.PasswordHash == hasPass)
                .Include(x => x.Roles).FirstOrDefaultAsync();
        }

        public async Task<List<User>> SearchAsync(string keyword, int index, int pageSize, bool desc = true)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await GetAllAsync(index, pageSize);
            }
            var query = _context.Users
                .Where(u => u.Username.Contains(keyword) || u.Email.Contains(keyword));
            if (desc)
            {
                query = query.OrderByDescending(u => u.CreatedAt);
            }
            else
            {
                query = query.OrderBy(u => u.CreatedAt);
            }
            return await query
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Roles)
                .ToListAsync();

        }
    }
}
