﻿namespace HackerNews.Integration.Interfaces
{
    public interface ICache
    {
        public Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> fetchFunction);
        public void Remove(string cacheKey);
    }
}
