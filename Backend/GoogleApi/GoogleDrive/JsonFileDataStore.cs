using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.GoogleDrive
{
    public class JsonFileDataStore : IDataStore
    {
        private readonly string _filePath;

        public JsonFileDataStore(string filePath)
        {
            _filePath = filePath;
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public Task ClearAsync()
        {
            if (File.Exists(_filePath)) File.Delete(_filePath);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            if (File.Exists(_filePath)) File.Delete(_filePath);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (!File.Exists(_filePath))
                return Task.FromResult(default(T));

            var json = File.ReadAllText(_filePath);
            var container = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (container != null && container.TryGetValue(key, out var v))
            {
                var serialized = JsonConvert.SerializeObject(v);
                var obj = JsonConvert.DeserializeObject<T>(serialized);
                return Task.FromResult(obj);
            }

            return Task.FromResult(default(T));
        }

        public Task StoreAsync<T>(string key, T value)
        {
            Dictionary<string, object> container = new();
            if (File.Exists(_filePath))
            {
                var existing = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(existing))
                    container = JsonConvert.DeserializeObject<Dictionary<string, object>>(existing) ?? new Dictionary<string, object>();
            }

            container[key] = value!;
            var json = JsonConvert.SerializeObject(container, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            return Task.CompletedTask;
        }
    }
}
