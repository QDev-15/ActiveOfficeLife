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

        public LogBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ActiveOfficeLifeDbContext>();
                var logRepo = scope.ServiceProvider.GetRequiredService<ILogRepository>();
                if (logRepo.TryDequeue(out var log))
                {
                    dbContext.Logs.Add(log);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(100, stoppingToken); // tránh CPU 100%
            }
        }
    }
}
