using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Services
{
    public class LogBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogRepository _logQueue;

        public LogBackgroundService(IServiceProvider serviceProvider, ILogRepository logQueue)
        {
            _serviceProvider = serviceProvider;
            _logQueue = logQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logQueue.TryDequeue(out var log))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ActiveOfficeLifeDbContext>();
                    dbContext.Logs.Add(log);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(100, stoppingToken); // tránh CPU 100%
            }
        }
    }
}
