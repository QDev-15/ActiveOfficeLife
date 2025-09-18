using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
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

        public async Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(PagingLogRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "timestamp", "Timestamp" },
                { "message", "Message" },
                { "stacktrace", "StackTrace" },
                { "source", "Source" },
                { "level", "Level" },

            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Timestamp"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            var query = _context.Logs.OrderByDescending(log => log.Timestamp).AsQueryable();

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                query = query.Where(log => log.Timestamp >= request.StartDate.Value && log.Timestamp <= request.EndDate.Value);
            }
            if (request.SearchText != null)
            {
                query = query.Where(log => log.Message.Contains(request.SearchText) || log.UserId!.Contains(request.SearchText) 
                        || log.IpAddress!.Contains(request.SearchText) || log.RequestPath!.Contains(request.SearchText) 
                        || log.Source!.Contains(request.SearchText) || log.StackTrace!.Contains(request.SearchText));
            }
            if (!string.IsNullOrEmpty(actualSortField))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, actualSortField));
                }
                else
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, actualSortField));
                }
            }
            var logs = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            var totalCount = await query.CountAsync();
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
