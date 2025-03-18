using HackerNews.Integration.Interfaces;
using HackerNews.Integration.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IHackerNews _hackerNews;

        public NewsController(IHackerNews hackerNews)
        {
            _hackerNews = hackerNews;
        }

        [HttpGet("topstories")]
        public async Task<IActionResult> GetTopStories([FromQuery] int page, [FromQuery] int pageSize )
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0");

            var stories = await _hackerNews.GetAllTopNews(page, pageSize);
            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalStories = stories.TotalStories,
                Data = stories.News
            });
        }
    }
}
