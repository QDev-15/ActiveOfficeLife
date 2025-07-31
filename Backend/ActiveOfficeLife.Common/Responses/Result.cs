namespace ActiveOfficeLife.Common.Responses
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
