using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface _IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid guidId);
        /// <summary>
        /// Find by Query: ex: .FindByQueryAsync(a => a.id = 123 && a.title = "name"))
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>List IEnumerable<T> </returns>
        Task<IEnumerable<T>> FindByQueryAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(T entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}
