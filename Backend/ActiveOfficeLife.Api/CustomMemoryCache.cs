using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api
{
    public  class CustomMemoryCache
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _keys = new();

        public CustomMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        //ex: _memoryCache.TryGetValue(cacheKey, out var cachedPost)
        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }
            value = default;
            return false;
        }


        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
            _keys.TryAdd(key, true);
        }    
        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(30));
            _keys.TryAdd(key, true);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        public void Clear()
        {
            foreach (var key in _keys.Keys)
            {
                _cache.Remove(key);
            }

            _keys.Clear();
        }
    }
}
