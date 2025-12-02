using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Common
{
    public static class AOLLogger
    {
        private static ILogService? LogService { get; set; }

        // Removed the static constructor with parameters as it is not allowed.
        // Instead, use a method to initialize the LogService.
        public static void Initialize(ILogService? logService)
        {
            LogService = logService;
        }

        public static void Info(LogProperties option)
        {
            LogService?.Info(option);
        }
        public static void Info(string messsage)
        {
            LogService?.Info(new LogProperties() { Message = messsage });
        }
        public static void Error(LogProperties option)
        {
            LogService?.Error(option);
        }
        public static void Error(Exception ex)
        {
            Error(new LogProperties()
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace
            });
        }
        public static void Error(string message)
        {
            Error(new LogProperties()
            {
                Message = message,
            });
        }
        public static void Error(string message, Exception ex)
        {
            Error(new LogProperties()
            {
                Message = message,
                Source = ex.Source,
                StackTrace = ex.StackTrace
            });
        }
        public static void Debug(LogProperties option)
        {
            LogService?.Debug(option);
        }
        public static void Trace(LogProperties option)
        {
            LogService?.Trace(option);
        }
    }
}
