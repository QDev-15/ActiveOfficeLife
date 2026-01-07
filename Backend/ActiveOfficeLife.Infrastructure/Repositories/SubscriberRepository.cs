using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class SubscriberRepository : _Repository<Subscriber>, ISubscriberRepository
    {
        public SubscriberRepository(ActiveOfficeLifeDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Subscriber?> GetByEmailAsync(string email)
        {
            var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
            return subscriber;
        }
    }
}
