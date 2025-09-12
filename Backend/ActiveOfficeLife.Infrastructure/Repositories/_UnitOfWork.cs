using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class _UnitOfWork : IUnitOfWork
    {
        private readonly ActiveOfficeLifeDbContext _context;
        public _UnitOfWork(ActiveOfficeLifeDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
