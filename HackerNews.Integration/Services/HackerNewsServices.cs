using HackerNews.Integration.Helpers;
using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Models;
using System.Net.Http;
using System.Text.Json;

namespace HackerNews.Integration.Services
{
    public class HackerNewsServices : IHackerNews
    {
        private readonly HttpClient _client;
        private readonly ICache _cache;

        public HackerNewsServices(HttpClient client, ICache cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<List<int>?> GetTopNewsIDs()
        {
            return await _cache.GetOrCreateAsync(
            AppConstants.TopStories,
            async () =>
            {
                var response = await _client.GetAsync("v0/topstories.json");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var newsIds= JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
                return newsIds.Take(200).ToList();
            },
            TimeSpan.FromMinutes(5) 
            );
        }

        public async Task<NewsResponse> GetAllTopNews(int page = 1, int pageSize = 10)
        {
            var topStoryIds = await GetTopNewsIDs();
           

            var pagedStoryIds = topStoryIds.OrderByDescending(x=>x)  //To Get Latest Stories
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToList();

            var newsTasks = pagedStoryIds.Select(async id => await GetNews(id));
            var newsList = await Task.WhenAll(newsTasks);

            return new NewsResponse()
            {
                News = newsList.Where(x=> !string.IsNullOrEmpty(x.Link))?.ToList(),
                TotalStories = topStoryIds.Count
            };
            //return newsList.Where(n => n != null).ToList();
        }

        public async Task<News?> GetNews(int newsId)
        {
            return await _cache.GetOrCreateAsync(
                AppConstants.News(newsId),
                async () =>
                {
                    var response = await _client.GetAsync($"v0/item/{newsId}.json");
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<News>(json);
                },
                 TimeSpan.FromMinutes(10)
            );
        }
    }
}
