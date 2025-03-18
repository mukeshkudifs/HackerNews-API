using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Services;
using System.Runtime.CompilerServices;

namespace HackerNews.API.Helpers
{
    public static class DependenciesHelper
    {
        public static void ConfigureHttpClient(this IServiceCollection services, string hackerNewsBaseUri)
        {
            if (!string.IsNullOrEmpty(hackerNewsBaseUri))
            {
                services.AddHttpClient<IHackerNews,HackerNewsServices>(client =>
                {
                    client.BaseAddress = new Uri(hackerNewsBaseUri);
                });
            }
        }


        public static void ConfigureServices(this IServiceCollection services)
        {
           services.AddSingleton<ICache,CacheService>();
        }
    }
}
