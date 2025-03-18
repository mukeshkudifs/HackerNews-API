using HackerNews.Integration.Models;

namespace HackerNews.Integration.Interfaces
{
    public interface IHackerNews
    {
        Task<List<int>?> GetTopNewsIDs();
        Task<News?> GetNews(int newsId);
        Task<NewsResponse> GetAllTopNews(int page = 1, int pageSize = 10);
    }
}
