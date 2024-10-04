using Endava.Data;
using Endava.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endava.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ArticlesController(ILogger<ArticlesController> logger, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("new")]
        public async Task<ActionResult<Article>> CreatePage(ArticleDto pageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var page = new Article
            {
                Id = pageDto.Id,
                Title = pageDto.Title,
                Author = pageDto.Author,
                Body = pageDto.Body,
            };

            _dbContext.Articles.Add(page);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPage), new { id = page.Id }, page);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ArticleDto>> GetPage(int id)
        {
            var article = await _dbContext.Articles.FindAsync(id);

            if (article is null)
            {
                return NotFound();
            }

            var pageDto = new ArticleDto
            {
                Id = article.Id,
                Author = article.Author,
                Body = article.Body,
                Title = article.Title
            };

            return pageDto;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<List<Article>> ListArticles()
        {
            var articlesFromDb = await _dbContext.Articles.ToListAsync();

            return articlesFromDb;
        }
    }
}
