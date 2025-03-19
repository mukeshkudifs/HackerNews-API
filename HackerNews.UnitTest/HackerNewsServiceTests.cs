using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Models;
using HackerNews.Integration.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Text.Json;
using HackerNews.Integration.Helpers;
using System.Linq;

namespace HackerNews.Tests
{
    [TestFixture]
    internal class HackerNewsServicesTests
    {
        public Mock<HttpMessageHandler> _mockHttpMessageHandler;
        public HttpClient _httpClient;
        private Mock<ICache> _mockCache;
        private HackerNewsServices _hackerNewsService;
        private Mock<HackerNewsServices> _mockHackerNewsServices;


        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://hacker-news.firebaseio.com/") };
            _mockCache = new Mock<ICache>();
            _hackerNewsService = new HackerNewsServices(_httpClient, _mockCache.Object);
            _mockHackerNewsServices =  new Mock<HackerNewsServices>(_httpClient, _mockCache.Object);
        }

        [Test]
        public async Task GetTopNewsIDs_ShouldReturnCachedData_WhenAvailable()
        {
            var cachedData = new List<int> { 1, 2, 3 };

            _mockCache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<int>>>>(), It.IsAny<TimeSpan?>()))
                      .ReturnsAsync(cachedData);

            var result = await _hackerNewsService.GetTopNewsIDs();

            Assert.That(result, Is.EqualTo(cachedData));

            _mockCache.Verify(c => c.GetOrCreateAsync(AppConstants.TopStories, It.IsAny<Func<Task<List<int>>>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Test]
        public async Task GetTopNewsIDs_ShouldFetchFromAPI_WhenCacheMisses()
        {
            var apiResponse = new List<int> { 4, 5, 6 };
            var jsonResponse = JsonSerializer.Serialize(apiResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            _mockCache
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<int>>>>(), It.IsAny<TimeSpan?>()))
                .Returns((string _, Func<Task<List<int>>> fetchFunc, TimeSpan? _) => fetchFunc());

            var result = await _hackerNewsService.GetTopNewsIDs();

            Assert.That(result, Is.EqualTo(apiResponse));
        }



        [Test]
        public void GetTopNewsIDs_ShouldThrowException_WhenAPIFails()
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockCache
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<int>>>>(), It.IsAny<TimeSpan?>()))
                .Returns((string _, Func<Task<List<int>>> fetchFunc, TimeSpan? _) => fetchFunc());

            Assert.ThrowsAsync<HttpRequestException>(async () => await _hackerNewsService.GetTopNewsIDs());
        }

        [Test]
        public async Task GetNews_ShouldReturnCachedData_WhenAvailable()
        {
            var newsId = 1;
            var cachedNews = new News { Id = newsId, Title = "Cached News" };

            _mockCache
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<News>>>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync(cachedNews);

            var result = await _hackerNewsService.GetNews(newsId);

            Assert.That(result, Is.EqualTo(cachedNews));
        }

        [Test]
        public async Task GetNews_ShouldFetchFromAPI_WhenCacheMisses()
        {
            var newsId = 2;
            var apiNews = new News { Id = newsId, Title = "Fetched News" };
            var jsonResponse = JsonSerializer.Serialize(apiNews);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            _mockCache
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<News>>>(), It.IsAny<TimeSpan?>()))
                .Returns((string _, Func<Task<News>> fetchFunc, TimeSpan? _) => fetchFunc());

            var result = await _hackerNewsService.GetNews(newsId);

            Assert.That(result.Id, Is.EqualTo(apiNews.Id));
            Assert.That(result.Title, Is.EqualTo(apiNews.Title));
        }

        [Test]
        public async Task GetAllTopNews_ShouldHandleEmptyNewsList()
        {
            _mockCache
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<int>>>>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync(new List<int>());

            var result = await _hackerNewsService.GetAllTopNews();

            Assert.That(result.News, Is.Empty);
            Assert.That(result.TotalStories, Is.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }
    }
}
