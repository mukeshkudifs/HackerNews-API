
namespace HackerNews.Integration.Helpers
{
    public static class AppConstants
    {
        public const string TopStories = "HackerNews:TopStories";
        public static string News(int storyID) => $"HackerNews:Story:{storyID}";
    }
}
