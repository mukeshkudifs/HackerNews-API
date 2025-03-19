namespace HackerNews.Integration.Interfaces
{
    public interface ICache
    {
        public Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> fetchFunction, TimeSpan? expiration = null);
        public void Remove(string cacheKey);
    }
}
