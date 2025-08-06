using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class LogRepository : _Repository<Log>, ILogRepository
    {
        private readonly ConcurrentQueue<Log> _queue = new();

        public LogRepository(ActiveOfficeLifeDbContext context, ConcurrentQueue<Log> queue)
            : base(context)
        {
            _queue = queue;
        }

        public void Enqueue(Log log) => _queue.Enqueue(log);

        public async Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
           var logs = await _context.Logs
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCount = await _context.Logs.CountAsync();
            return (logs.Select(log => new LogModel()
            {
                Id = log.Id.ToString(),
                Message = log.Message,
                Level = log.Level.ToString(),
                Timestamp = log.Timestamp,
                UserId = log.UserId,
                IpAddress = log.IpAddress,
                RequestPath = log.RequestPath,
                Source = log.Source,
                StackTrace = log.StackTrace
            }), totalCount);
        }

        public async Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var logs = await _context.Logs
                .Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCount = await _context.Logs.CountAsync(log => log.Timestamp >= startDate && log.Timestamp <= endDate);
            return (logs.Select(log => new LogModel()
            {
                Id = log.Id.ToString(),
                Message = log.Message,
                Level = log.Level.ToString(),
                Timestamp = log.Timestamp,
                UserId = log.UserId,
                IpAddress = log.IpAddress,
                RequestPath = log.RequestPath,
                Source = log.Source,
                StackTrace = log.StackTrace
            }), totalCount);
        }

        public bool TryDequeue(out Log log) => _queue.TryDequeue(out log);
    }
}
