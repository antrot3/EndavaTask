using Common.Models;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Service.Interfaces;
using System.Security.Claims;

namespace ServiceLayer.Service
{
    public class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext _context;

        public ArticleService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<ArticlesDto>> CreateNewArticleAsync(ArticleCreateDto articleDto, ClaimsPrincipal user)
        {
            var userName = user.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (currentUser == null)
                throw new Exception("User not found");

            var article = new Article
            {
                Title = articleDto.Title,
                Author = currentUser,
                Body = articleDto.Body,
                UserId = currentUser.Id
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return new ArticlesDto
            {
                Id = article.Id,
                Author = article.Author.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        public ActionResult<ArticlesDto> EditArticle(ArticleEditDto articleEditDto)
        {
            var article = _context.Articles.FirstOrDefault(x => x.Id == articleEditDto.ArticleId);
            if (article == null)
                throw new Exception("Article does not exist");

            article.Title = articleEditDto.Title;
            article.Body = articleEditDto.Body;
            _context.SaveChanges();

            var author = _context.Users.FirstOrDefault(x => x.Id == article.UserId);
            return new ArticlesDto
            {
                Id = article.Id,
                Author = author?.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        public async Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article is null)
            {
                throw new Exception("Article not found");
            }

            var author = await _context.Users.FirstOrDefaultAsync(x => x.Id == article.UserId);
            return new ArticlesDto
            {
                Id = article.Id,
                Author = author?.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        public async Task<List<ArticlesDto>> GetAllArticlesAsync()
        {
            var articlesFromDb = await _context.Articles.ToListAsync();
            var articles = new List<ArticlesDto>();

            foreach (var article in articlesFromDb)
            {
                var author = await _context.Users.FirstOrDefaultAsync(x => x.Id == article.UserId);
                var articlesDto = new ArticlesDto
                {
                    Id = article.Id,
                    Author = author?.UserName,
                    Body = article.Body,
                    Title = article.Title
                };
                articles.Add(articlesDto);
            }

            return articles;
        }
    }
}