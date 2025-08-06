using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ILogRepository : _IRepository<Log>
    {
        void Enqueue(Log log);
        bool TryDequeue(out Log log);
        // get all logs paginated
        Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(int pageNumber, int pageSize);
        // get logs by start date and end date
        Task<(IEnumerable<LogModel> Items, int totalCount)> GetAllAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    }
}
