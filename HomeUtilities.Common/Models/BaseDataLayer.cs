using HomeUtilities.Common.Attributes;
using HomeUtilities.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeUtilities.Common.Models
{
    public class BaseDataLayer
    {
        protected IMemoryCache _cache;
        protected ILogger<BaseDataLayer> _logger;

        public BaseDataLayer(ILogger<BaseDataLayer> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        protected async Task<T> GetCachedResponseAsync<T>(string cacheKey, Func<Task<T>> factory, MemoryCacheEntryOptions cacheOptions = null) where T : IBaseResponse, new()
        {
            if (_cache.TryGetValue(cacheKey, out T cachedResponse))
            {
                _logger.LogInformation($"Cache hit for key: {cacheKey}");
                return cachedResponse;
            }

            _logger.LogInformation($"Cache miss for key: {cacheKey}, fetching from source.");
            var response = await factory();

            if (response != null && response.Success) // Only cache successful responses
            {
                _cache.Set(cacheKey, response, cacheOptions ?? new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5))); // Default sliding expiration
            }

            return response;
        }

        protected MemoryCacheEntryOptions GetMethodCachingOptions(MethodBase methodInfo)
        {
            var cacheableAttribute = methodInfo.GetCustomAttribute<CacheableAttribute>();
            if (cacheableAttribute == null)
            {
                return null;
            }

            var cacheOptions = new MemoryCacheEntryOptions();
            if (cacheableAttribute.AbsoluteExpiration.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(cacheableAttribute.AbsoluteExpiration.Value);
            }
            if (cacheableAttribute.SlidingExpiration.HasValue)
            {
                cacheOptions.SetSlidingExpiration(cacheableAttribute.SlidingExpiration.Value);
            }

            return cacheOptions;
        }
    }
}
