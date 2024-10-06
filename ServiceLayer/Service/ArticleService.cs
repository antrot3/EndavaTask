using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Service.Interfaces;
using System.Security.Claims;

namespace ServiceLayer.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IDatabaseService _dbContextService;

        public ArticleService(IDatabaseService dbContextService)
        {
            _dbContextService = dbContextService;
        }

        public async Task<ActionResult<ArticlesDto>> CreateNewArticleAsync(ArticleCreateDto articleDto, ClaimsPrincipal user)
        {
            ValidateUserAndArticleDto(user, articleDto);

            var currentUser = await GetCurrentUserAsync(user);
            var article = CreateArticleEntity(articleDto, currentUser);

            await SaveArticleAsync(article);

            return await CreateArticleDto(article);
        }

        public async Task<ActionResult<ArticlesDto>> EditArticleAsync(ArticleEditDto articleEditDto)
        {
            ValidateArticleEditDto(articleEditDto);

            var article = await GetArticleAndCheckIsItNull(articleEditDto.ArticleId.Value);
            UpdateArticleEntity(article, articleEditDto);

            await SaveChangesAsync();

            return await CreateArticleDto(article);
        }

        public async Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id)
        {
            ValidateArticleId(id);

            var article = await GetArticleAndCheckIsItNull(id);

            return await CreateArticleDto(article);
        }

        public async Task<ActionResult<ArticlesDto>> DeleteArticleByIdAsync(int id)
        {
            ValidateArticleId(id);

            return await CreateArticleDto(await _dbContextService.DeleteArticleByIdAsync(id));
        }

        public async Task<ActionResult<List<ArticlesDto>>> GetAllArticlesAsync()
        {
            var articlesFromDb = await _dbContextService.GetAllArticlesAsync();
            return articlesFromDb?.Select(article => CreateArticleDto(article).Result).ToList() ?? new List<ArticlesDto>();
        }

        private void ValidateUserAndArticleDto(ClaimsPrincipal user, ArticleCreateDto articleDto)
        {
            if (string.IsNullOrEmpty(user?.Identity?.Name))
                throw new ArgumentException("User identity is invalid");

            ValidateArticleDto(articleDto);
        }

        private void ValidateArticleDto(ArticleCreateDto articleDto)
        {
            if (articleDto == null)
                throw new ArgumentNullException(nameof(articleDto));

            if (string.IsNullOrEmpty(articleDto.Title) || string.IsNullOrEmpty(articleDto.Body))
                throw new ArgumentException("Title and Body are required for an article");
        }

        private void ValidateArticleEditDto(ArticleEditDto articleEditDto)
        {
            if (articleEditDto == null || articleEditDto.ArticleId == null)
                throw new ArgumentNullException(nameof(articleEditDto), "Article ID is required");

            ValidateArticleDto(new ArticleCreateDto { Title = articleEditDto.Title, Body = articleEditDto.Body });
        }

        private void ValidateArticleId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid article ID");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userName = user.Identity?.Name;
            var currentUser = await _dbContextService.GetCurrentUserAsync(userName);

            if (currentUser == null || string.IsNullOrEmpty(currentUser.Id))
                throw new Exception("User not found or invalid");

            return currentUser;
        }

        private Article CreateArticleEntity(ArticleCreateDto articleDto, ApplicationUser currentUser)
        {
            return new Article
            {
                Title = articleDto.Title,
                Author = currentUser,
                Body = articleDto.Body,
                UserId = currentUser.Id
            };
        }

        private void UpdateArticleEntity(Article article, ArticleEditDto articleEditDto)
        {
            article.Title = articleEditDto.Title;
            article.Body = articleEditDto.Body;
        }

        private async Task SaveArticleAsync(Article article)
        {
            try
            {
                await _dbContextService.AddArticleAndSaveChangesAsync(article);
            }
            catch (DbUpdateException)
            {
                throw new Exception("Error saving article to the database");
            }
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                await _dbContextService.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new Exception("Error saving changes to the database");
            }
        }

        private async Task<Article> GetArticleAndCheckIsItNull(int id)
        {
            var article = await _dbContextService.GetArticleByIdAsync(id);

            if (article == null)
                throw new Exception("Article not found");

            return article;
        }

        private async Task<ArticlesDto> CreateArticleDto(Article article)
        {
            var author = await _dbContextService.GetUserByIdAsync(article.UserId);

            if (author == null || string.IsNullOrEmpty(author.UserName))
                throw new Exception("Author not found or invalid");

            return new ArticlesDto
            {
                Id = article.Id,
                Author = author.UserName,
                Body = article.Body,
                Title = article.Title
            };
        }
    }
}