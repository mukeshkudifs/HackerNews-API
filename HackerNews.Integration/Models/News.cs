
using System.Text.Json.Serialization;

namespace HackerNews.Integration.Models
{
    public class News
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public required string Title { get; set; }
        [JsonPropertyName("url")]
        public string? Link { get; set; }

    }

    public class NewsResponse
    {
        public required List<News> News { get; set; }
        public int TotalStories { get; set; }
    }
}
