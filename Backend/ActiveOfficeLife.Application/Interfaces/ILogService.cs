using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ILogService
    {
        void Info(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null);
        void Error(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null);
        void Debug(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null);
        void Trace(string message, string? source = null, string? userId = null, string? stackTrace = null, string? ipAddress = null, string? requestPath = null);
    }
}
