namespace ActiveOfficeLife.Admin.Interfaces
{
    public interface IApiService
    {
        Task<HttpResponseMessage?> GetAsync(string endpoint);
        Task<HttpResponseMessage?> PostAsync(string endpoint, object data);
        Task<HttpResponseMessage?> PutAsync(string endpoint, object data);
        Task<HttpResponseMessage?> PatchAsync(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
    }

}
