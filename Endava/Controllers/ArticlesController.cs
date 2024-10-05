using Endava.Data;
using Endava.Models;
using Endava.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<ActionResult<ArticlesDto>> CreateArticle(ArticleCreateDto artilceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userName = User.Identity.Name;
            var currentUser = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);

            if (currentUser == null)
                return BadRequest("User can not be found");

            var article = new Article
            {
                Title = artilceDto.Title,
                Author = currentUser,
                Body = artilceDto.Body,
            };
            article.UserId = currentUser.Id;
            _dbContext.Articles.Add(article);
            await _dbContext.SaveChangesAsync();

            return new ArticlesDto
            {
                Id = article.Id,
                Author = article.Author.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit")]
        public async Task<ActionResult<ArticlesDto>> EditArticle( ArticleEditDto articleEditDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var article = _dbContext.Articles.FirstOrDefault(x => x.Id == articleEditDto.ArticleId);
            if (article == null)
                return BadRequest("Article does not exits");

            article.Title = articleEditDto.Title;
            article.Body = articleEditDto.Body;
            _dbContext.SaveChanges();
            var author = _dbContext.Users.FirstOrDefault(x => x.Id == article.UserId);
            return new ArticlesDto
            {
                Id = article.Id,
                Author = author.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ArticlesDto>> GetArticleById(int id)
        {
            var article = await _dbContext.Articles.FindAsync(id);

            if (article is null)
            {
                return BadRequest("Article is not found");
            }
            var author = _dbContext.Users.FirstOrDefault(x => x.Id == article.UserId);
            var articlesDto = new ArticlesDto
            {
                Id = article.Id,
                Author = author.UserName,
                Body = article.Body,
                Title = article.Title
            };

            return articlesDto;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<List<ArticlesDto>> ListArticles()
        {
            var articlesFromDb = await _dbContext.Articles.ToListAsync();
            var articles = new List<ArticlesDto>();
            foreach (var article in articlesFromDb)
            {
                var author = _dbContext.Users.FirstOrDefault(x => x.Id == article.UserId);
                var articlesDto = new ArticlesDto
                {
                    Id = article.Id,
                    Author = author.UserName,
                    Body = article.Body,
                    Title = article.Title
                };
                articles.Add(articlesDto);
            }

            return articles;
        }
    }
}
