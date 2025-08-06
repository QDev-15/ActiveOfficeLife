using ActiveOfficeLife.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Common
{
    public static class AOLLogger
    {
        public static ILogService? LogService { get; set; }

        // Removed the static constructor with parameters as it is not allowed.
        // Instead, use a method to initialize the LogService.
        public static void Initialize(ILogService? logService)
        {
            LogService = logService;
        }

        public static void Info(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            LogService?.Info(message, source, userId, stackTrace, ipAddress, requestPath);
        }
        public static void Error(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            LogService?.Error(message, source, userId, stackTrace, ipAddress, requestPath);
        }
        public static void Debug(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            LogService?.Debug(message, source, userId, stackTrace, ipAddress, requestPath);
        }
        public static void Trace(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null)
        {
            LogService?.Trace(message, source, userId, stackTrace, ipAddress, requestPath);
        }
    }
}
