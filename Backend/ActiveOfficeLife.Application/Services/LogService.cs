using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }

        private Log GetLog(LogLevel level, string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            return new Log
            {
                Id = Guid.NewGuid(),
                Level = level,
                Message = message,
                Source = source,
                UserId = userId,
                IpAddress = ipAddress,
                RequestPath = requestPath,
                Timestamp = DateTime.UtcNow,
                StackTrace = stackTrace
            };
        }
        public void Debug(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            var log = GetLog(LogLevel.Debug, message, source, userId, stackTrace, ipAddress, requestPath);
            _logRepository.Enqueue(log);
        }

        public void Error(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            var log = GetLog(LogLevel.Error, message, source, userId, stackTrace, ipAddress, requestPath);
            _logRepository.Enqueue(log);
        }

        public void Info(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            var log = GetLog(LogLevel.Information, message, source, userId, stackTrace, ipAddress, requestPath);
            _logRepository.Enqueue(log);
        }

        public void Trace(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            var log = GetLog(LogLevel.Trace, message, source, userId, stackTrace, ipAddress, requestPath);
            _logRepository.Enqueue(log);
        }
    }
}
