namespace ActiveOfficeLife.Admin.Interfaces
{
    public interface IApiService
    {
        Task<HttpResponseMessage?> GetAsync(string endpoint);
        Task<HttpResponseMessage?> GetAsync(string endpoint, string? token);
        Task<HttpResponseMessage?> PostAsync(string endpoint, object data);
        Task<HttpResponseMessage?> PostAsync(string endpoint, object data, string? token);
        Task<HttpResponseMessage?> PutAsync(string endpoint, object data);
        Task<HttpResponseMessage?> PutAsync(string endpoint, object data, string? token);
        Task<HttpResponseMessage?> PatchAsync(string endpoint, object data);
        Task<HttpResponseMessage?> PatchAsync(string endpoint, object data, string? token);
        Task<bool> DeleteAsync(string endpoint);
        Task<bool> DeleteAsync(string endpoint, string? token);
    }

}
