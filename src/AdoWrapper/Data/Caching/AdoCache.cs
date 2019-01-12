using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;

namespace AdoWrapper.Data.Caching
{
    public class AdoCache
    {
        private MemoryCache cache = MemoryCache.Default;

        public void Add(string key, object value, double duration = 30)
        {
            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(duration)
            };

            cache.Add(key, value, policy);
        }

        public T GetOrAdd<T>(string key, Func<T> value, double duration = 30)
        {
            T cacheItem = Get<T>(key);
            if (cacheItem != null)
            {
                return cacheItem;
            }
            else
            {
                if (value != null)
                {
                    cacheItem = value.Invoke();
                    Add(key, cacheItem, duration);
                }
            }
            return cacheItem;
        }

        public T Get<T>(string key)
        {
            object cacheItem = cache.Get(key);
            if (cacheItem != null)
            {
                return (T)cacheItem;
            }
            return default(T);
        }
    }
}
