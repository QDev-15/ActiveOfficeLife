using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// successfully return model from HttpResponseMessage, fail return null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<T?> ToModelAsync<T>(this HttpResponseMessage? response)
        {
            if (response == null || !response.IsSuccessStatusCode)
                return default;

            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
                return default;

            using var doc = JsonDocument.Parse(body);

            // Nếu API có dạng { "data": { "items": [...] } }
            if (doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.TryGetProperty("items", out var itemsElement))
                {
                    return JsonSerializer.Deserialize<T>(itemsElement.ToString(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // Nếu chỉ có data (không có items)
                return JsonSerializer.Deserialize<T>(dataElement.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Nếu API trả thẳng object JSON
            return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
