namespace ActiveOfficeLife.Common.Responses
{
    public class ResultError : Result
    {
        public ResultError()
        {
            Success = false;
        }
        public ResultError(string message)
        {
            Success = false;
            Errors = (new Error {Message = message });
        }
        public ResultError(string message, string code)
        {
            Success = false;
            Errors = (new Error { Code = code, Message = message });
        }
        public ResultError(Exception ex)
        {
            Success = false;
            Errors = (new Error { Message = ex.Message, Source = ex.Source });
        }
        public ResultError(Error err)
        {
            Success = false;
            Errors = (err);
        }
    }
}
