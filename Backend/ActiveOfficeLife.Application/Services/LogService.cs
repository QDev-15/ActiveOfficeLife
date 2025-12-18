using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogService(ILogRepository logRepository, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }
        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                   ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value
                   ?? _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value
                   ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? "anonymous";
        }
        public string GetIpAddress()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            } catch (Exception ex)
            {
                return "unknown";
            }
            
        }
        public string GetPath()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path.Value
                   ?? "";
        }
        private Log GetLog(LogLevel level, LogProperties option)
        {
            return new Log
            {
                Id = Guid.NewGuid(),
                Level = level,
                Message = option.Message ?? "unknown message!",
                Source = option.Source,
                UserId = option.UserId ?? this.GetUserId(),
                IpAddress = option.IpAddress ?? this.GetIpAddress(),
                RequestPath = option.RequestPath ?? this.GetPath(),
                Timestamp = DateTime.UtcNow,
                StackTrace = option.StackTrace
            };
        }
        public void Debug(LogProperties option)
        {
            var log = GetLog(LogLevel.Debug, option);
            _logRepository.Enqueue(log);
        }

        public void Error(LogProperties option)
        {
            var log = GetLog(LogLevel.Error, option);
            _logRepository.Enqueue(log);
        }

        public void Info(LogProperties option)
        {
            var log = GetLog(LogLevel.Information, option);
            _logRepository.Enqueue(log);
        }

        public void Trace(LogProperties option)
        {
            var log = GetLog(LogLevel.Trace, option);
            _logRepository.Enqueue(log);
        }

        public void Warn(LogProperties option)
        {
            var log = GetLog(LogLevel.Warning, option);
            _logRepository.Enqueue(log);
        }
    }
}
