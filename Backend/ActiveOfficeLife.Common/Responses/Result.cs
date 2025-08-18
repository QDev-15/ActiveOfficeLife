namespace ActiveOfficeLife.Common.Responses
{
    public class Error
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; } = null;
        public string? Source { get; set; } = string.Empty;
        public string? StackTrace { get; set; } = null;
        public string? Field { get; set; } = null;
        public object? Data { get; set; } = null;

    }
    public class Result
    {
        public bool Success { get; set; } = true;
        public object Data { get; set; } = null;
        public Error Errors { get; set; } = new Error();
        public Result() { 
        
        }
    }
}
