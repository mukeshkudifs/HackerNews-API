using HackerNews.Integration.Helpers;
using NUnit.Framework;

namespace HackerNews.UnitTests.Helpers
{
    [TestFixture]
    public class AppConstantsTests
    {
        [Test]
        public void TopStories_ShouldHaveCorrectValue()
        {
            var expectedValue = "HackerNews:TopStories";

            Assert.That(AppConstants.TopStories, Is.EqualTo(expectedValue));
        }



        [TestCase(1, "HackerNews:Story:1")]
        [TestCase(100, "HackerNews:Story:100")]
        [TestCase(1000, "HackerNews:Story:1000")]
        public void News_ShouldReturnCorrectFormat(int newsId,string expectedValue)
        {
            string actual = AppConstants.News(newsId);


            Assert.That(actual, Is.EqualTo(expectedValue));
        }

    }
}
