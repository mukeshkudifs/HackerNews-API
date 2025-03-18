using HackerNews.API.Helpers;
using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;

namespace HackerNews.Tests.Helpers
{
    [TestFixture]
    public class DependenciesHelperTests
    {
        private IServiceCollection _services;

        [SetUp]
        public void SetUp()
        {
            _services = new ServiceCollection();
        }

        [Test]
        public void ConfigureHttpClient_ShouldRegisterHttpClient_WhenBaseUriIsValid()
        {
            string validUri = "https://hacker-news.firebaseio.com/v0/";
            
            _services.AddMemoryCache();
            _services.AddScoped<HttpClient>();
            _services.AddSingleton<ICache, CacheService>();
            _services.AddHttpClient<IHackerNews, HackerNewsServices>(client =>
            {
                client.BaseAddress = new Uri(validUri);
            });

      

            _services.ConfigureHttpClient(validUri);
            var provider = _services.BuildServiceProvider();

            var httpClientFactory = provider.GetService<IHttpClientFactory>();
            ClassicAssert.IsNotNull(httpClientFactory, "HttpClientFactory should be registered.");

            var hackerNewsService = provider.GetService<IHackerNews>();
            ClassicAssert.IsNotNull(hackerNewsService, "IHackerNews should be registered.");
            ClassicAssert.IsInstanceOf<HackerNewsServices>(hackerNewsService, "Registered service should be of type HackerNewsServices.");
        }

        [Test]
        public void ConfigureHttpClient_ShouldNotRegisterHttpClient_WhenBaseUriIsNull()
        {
            string invalidUri = null;

            _services.ConfigureHttpClient(invalidUri);
            var provider = _services.BuildServiceProvider();

            var hackerNewsService = provider.GetService<IHackerNews>();
            ClassicAssert.IsNull(hackerNewsService, "IHackerNews should not be registered when URI is null.");
        }

        [Test]
        public void ConfigureHttpClient_ShouldNotRegisterHttpClient_WhenBaseUriIsEmpty()
        {
            string emptyUri = "";

            _services.ConfigureHttpClient(emptyUri);
            var provider = _services.BuildServiceProvider();

            var hackerNewsService = provider.GetService<IHackerNews>();
            ClassicAssert.IsNull(hackerNewsService, "IHackerNews should not be registered when URI is empty.");
        }

        [Test]
        public void ConfigureServices_ShouldRegisterCacheService()
        {
            _services.AddMemoryCache();  
            _services.AddSingleton<ICache, CacheService>(); 

            _services.ConfigureServices();
            var provider = _services.BuildServiceProvider();

            var cacheService = provider.GetService<ICache>();
            ClassicAssert.IsNotNull(cacheService, "ICache should be registered.");
            ClassicAssert.IsInstanceOf<CacheService>(cacheService, "Registered service should be of type CacheService.");
        }
    }
}
