using HackerNews.Integration.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace HackerNews.Tests
{
    [TestFixture]
    internal class CacheServiceTests
    {
        private Mock<IMemoryCache> _mockMemoryCache;
        private CacheService _cacheService;

        [SetUp]
        public void Setup()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();
            _cacheService = new CacheService(_mockMemoryCache.Object);
        }

        [Test]
        public async Task GetOrCreateAsync_ShouldReturnCachedValue_WhenValueExists()
        {
            string cacheKey = "test_key";
            string expectedValue = "cached_data";

            object cacheEntry = expectedValue;
            _mockMemoryCache
                .Setup(mc => mc.TryGetValue(cacheKey, out cacheEntry))
                .Returns(true); 

            var result = await _cacheService.GetOrCreateAsync(cacheKey, () => Task.FromResult("new_data"));

            ClassicAssert.AreEqual(expectedValue, result);
            _mockMemoryCache.Verify(mc => mc.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task GetOrCreateAsync_ShouldFetchNewValue_WhenCacheIsEmpty()
        {
            string cacheKey = "test_key";
            string expectedValue = "new_data";
            object cacheEntry = null;

            _mockMemoryCache
                .Setup(mc => mc.TryGetValue(cacheKey, out cacheEntry))
                .Returns(false);

            var mockCacheEntry = new Mock<ICacheEntry>();
            _mockMemoryCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var result = await _cacheService.GetOrCreateAsync(cacheKey, () => Task.FromResult(expectedValue));

            ClassicAssert.AreEqual(expectedValue, result);
            _mockMemoryCache.Verify(mc => mc.CreateEntry(cacheKey), Times.Once);
        }

        [Test]
        public void Remove_ShouldCallMemoryCacheRemove()
        {
            string cacheKey = "test_key";

            _cacheService.Remove(cacheKey);

            _mockMemoryCache.Verify(mc => mc.Remove(cacheKey), Times.Once);
        }
    }
}
