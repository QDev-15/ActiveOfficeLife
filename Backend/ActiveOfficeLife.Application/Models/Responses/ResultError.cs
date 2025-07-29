using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Responses
{
    public class ResultError : Result
    {
        public string Code { get; set; } = string.Empty;
        public ResultError()
        {
            IsSuccess = false;
        }
        public ResultError(string message)
        {
            IsSuccess = false;
            Message = message;
        }
        public ResultError(string code, string message)
        {
            IsSuccess = false;
            Code = code;
            Message = message;
        }
        public ResultError(string code, string message, object data) : this(code, message)
        {
            IsSuccess = false;
            Data = data;
        }
        public ResultError(string code, string message, Exception ex) : this(code, message)
        {
            IsSuccess = false;
            Data = new { Exception = ex.ToString() };
        }

        public ResultError(Exception ex)
        {
            IsSuccess = false;
            Code = "500"; // Internal Server Error
            Message = ex.Message;
        }
    }
}
