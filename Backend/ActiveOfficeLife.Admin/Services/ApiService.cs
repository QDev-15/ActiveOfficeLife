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
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            baseApi = configuration.GetSection("BaseApi").Get<BaseApi>();
        }

        private HttpClient CreateClient(string? token)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseApi.Url);

            token = token ?? _httpContextAccessor.HttpContext?.Request.Cookies[baseApi.AccessToken];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseApi.Url);

            var token = _httpContextAccessor.HttpContext?.Request.Cookies[baseApi.AccessToken];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<HttpResponseMessage?> GetAsync(string endpoint, string? token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync(endpoint);
            return response;
        }
        public async Task<HttpResponseMessage?> GetAsync(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);
            return response;
        }

        public async Task<HttpResponseMessage?> PostAsync(string endpoint, object data)
        {
            var client = CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            return response;
        }
        public async Task<HttpResponseMessage?> PostAsync(string endpoint, object data, string? token)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            return response;
        }

        public async Task<HttpResponseMessage?> PutAsync(string endpoint, object data)
        {
            var client = CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);

            return response;
        }
        public async Task<HttpResponseMessage?> PutAsync(string endpoint, object data, string? token)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);

            return response;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteAsync(string endpoint, string? token)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage?> PatchAsync(string endpoint, object data)
        {
            var client = CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
            {
                Content = content
            };
            var response = await client.SendAsync(request);
            return response;
        }
        public async Task<HttpResponseMessage?> PatchAsync(string endpoint, object data, string? token)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
            {
                Content = content
            };
            var response = await client.SendAsync(request);
            return response;
        }
    }

}
