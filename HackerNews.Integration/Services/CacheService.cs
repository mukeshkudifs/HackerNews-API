using HackerNews.Integration.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNews.Integration.Services
{
    public class CacheService : ICache
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> fetchFunction)
        {
            var cachedData = await _cache.GetOrCreateAsync(cacheKey, async entry =>
              {
                  return await fetchFunction().ConfigureAwait(false);
              }).ConfigureAwait(false);

            return cachedData;
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}
