using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Data;
using ServiceLayer.Models;
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

        public async Task<ActionResult<ArticlesDto>> CreateNewArticleAsync(ArticleCreateDto artilceDto, ClaimsPrincipal user)
        {
            var userName = user.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(x => x.UserName == userName);

            if (currentUser == null)
                throw new Exception("User not found");

            var article = new Article
            {
                Title = artilceDto.Title,
                Author = currentUser,
                Body = artilceDto.Body,
            };
            article.UserId = currentUser.Id;
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
                Author = author.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }

        public async Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article is null)
            {
                throw new Exception("Article is not found");
            }
            var author = _context.Users.FirstOrDefault(x => x.Id == article.UserId);
            var articlesDto = new ArticlesDto
            {
                Id = article.Id,
                Author = author.UserName,
                Body = article.Body,
                Title = article.Title
            };

            return articlesDto;
        }
    }
}
