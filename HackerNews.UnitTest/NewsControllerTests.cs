using HackerNews.API.Controllers;
using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace HackerNews.UnitTest
{
    [TestFixture]
    internal class NewsControllerTests
    {

        private Mock<IHackerNews> _moqHackerNews;
        private NewsController _newsController;


        [SetUp]
        public void setup()
        {
            _moqHackerNews = new Mock<IHackerNews>();
            _newsController = new NewsController(_moqHackerNews.Object);
        }

        [TestCase]
        public async Task GetTopStories_ShouldReturnsPaginatedResponse()
        {
            int page = 1, pageSize = 2;
            int totalStories = 10;

            var mockNewsResponse = new NewsResponse()
            {
                News = new List<News>()
                        {
                            new(){Id=1,Title=$"title{1}" },
                            new(){Id=1,Title=$"title{1}" }
                        },
                TotalStories = 2
            };



            _moqHackerNews.Setup(x => x.GetAllTopNews(page, pageSize))
                .ReturnsAsync(mockNewsResponse);

            var response = await _newsController.GetTopStories(page, pageSize);
           
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var jsonString = JsonConvert.SerializeObject(okResult.Value); 
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            Assert.That(responseData["Page"], Is.EqualTo(page));
            Assert.That(responseData["PageSize"], Is.EqualTo(pageSize));
            Assert.That(responseData["TotalStories"], Is.EqualTo(mockNewsResponse.TotalStories));
        }

        [Test]
        [TestCase(0, 20)]
        [TestCase(-3, 5)]
        [TestCase(1, -3)]
        [TestCase(10, 0)]
        [TestCase(0, 0)]

        public async Task GetTopStories_InvalidPageOrPageSize_ReturnsBadRequest(int page, int pageSize)
        {
            // Act
            var result = await _newsController.GetTopStories(page, pageSize);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Page and pageSize must be greater than 0"));
        }


    }
}
