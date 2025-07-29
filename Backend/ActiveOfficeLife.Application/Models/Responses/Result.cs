using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Responses
{
    public class Result
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; } = null;
        public Result() { 
        
        }
    }
}
