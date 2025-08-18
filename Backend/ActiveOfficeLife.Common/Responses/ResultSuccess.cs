namespace ActiveOfficeLife.Common.Responses
{
    public class ResultSuccess : Result
    {
        public ResultSuccess() { 
            this.Success = true;
        }
        public ResultSuccess(object data) : base()
        {
            this.Data = data;
            this.Success = true;
        }
    }
}
