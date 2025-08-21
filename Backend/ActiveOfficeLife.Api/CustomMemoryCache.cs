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

        /// <summary>
        /// get a value from the cache by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        //ex: _memoryCache.TryGetValue(cacheKey, out var cachedPost)
        /// <summary>
        /// try to get a value from the cache by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }
            value = default;
            return false;
        }


        /// <summary>
        /// set a key-value pair in the cache with a specified expiration time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
            _keys.TryAdd(key, true);
        }
        /// <summary>
        /// set a key-value pair in the cache with a default expiration time of 30 minutes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(30));
            _keys.TryAdd(key, true);
        }

        /// <summary>
        /// remove a key from the cache
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        /// <summary>
        /// remove all keys from the cache
        /// </summary>
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
