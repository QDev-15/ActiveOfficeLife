using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ActiveOfficeLifeDbContext _context;
        private readonly ConcurrentQueue<Log> _queue = new();
        public LogRepository(ActiveOfficeLifeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Enqueue(Log log) => _queue.Enqueue(log);

        public bool TryDequeue(out Log log) => _queue.TryDequeue(out log);
        public async Task SaveLogAsync(Log log)
        {
            _context.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
