using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ActiveOfficeLife.Admin.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly BaseApi baseApi; 

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            baseApi = configuration.GetSection("BaseApi").Get<BaseApi>();
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseApi.Url);

            var token = _httpContextAccessor.HttpContext?.Session.GetString(baseApi.AccessToken);
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }

}
