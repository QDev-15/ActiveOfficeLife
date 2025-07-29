using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Responses
{
    public class ResultSuccess : Result
    {
        public ResultSuccess() { 
            this.IsSuccess = true;
        }
        public ResultSuccess(string message) : base()
        {
            this.Message = message;
            this.IsSuccess = true;
        }
        public ResultSuccess(object data) : base()
        {
            this.Data = data;
            this.IsSuccess = true;
        }
        public ResultSuccess(string message, object data) : base()
        {
            this.Message = message;
            this.Data = data;
            this.IsSuccess = true;
        }
    }
}
