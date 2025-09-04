using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.Responses
{
    public class ResultResponse
    {
        public ResultResponse(bool isSuccess) {
            IsSuccess = isSuccess;
        }
        public ResultResponse(bool isSuccess, string message) {
            IsSuccess = isSuccess;
            Message = message;
        }
        public ResultResponse(bool isSuccess, Exception exception) {
            IsSuccess = isSuccess;
            Exception = exception;
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            InnerExceptionMessage = exception.InnerException?.Message;
        }
        public ResultResponse(bool isSuccess, string message, Exception exception) {
            IsSuccess = isSuccess;
            Message = message;
            Exception = exception;
            StackTrace = exception.StackTrace;
            InnerExceptionMessage = exception.InnerException?.Message;
        }

        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; } = null;
        public Exception? Exception { get; set; } = null;
        public string? StackTrace { get; set; } = null;
        public string? InnerExceptionMessage { get; set; } = null;
        public int? StatusCode { get; set; } = null;
        public string? StatusDescription { get; set; } = null;
        public string? StackTraceDescription { get; set; } = null;
    }
}
