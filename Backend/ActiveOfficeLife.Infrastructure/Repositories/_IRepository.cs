using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class _Repository<T> : _IRepository<T> where T : class
    {
        public readonly ActiveOfficeLifeDbContext _context;
        private readonly DbSet<T> _dbSet;

        public _Repository(ActiveOfficeLifeDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> FindByQueryAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid guidId)
        {
            return await _dbSet.FindAsync(guidId);
        }

        public void RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRangeAsync(T entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
