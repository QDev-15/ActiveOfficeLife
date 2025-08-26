using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ILogRepository : _IRepository<Log>
    {
        void Enqueue(Log log);
        bool TryDequeue(out Log log);
        // get all logs paginated
        Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(PagingLogRequest request);
    }
}
