using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ILogRepository
    {
        void Enqueue(Log log);
        bool TryDequeue(out Log log);
        Task SaveLogAsync(Log log);
    }
}
