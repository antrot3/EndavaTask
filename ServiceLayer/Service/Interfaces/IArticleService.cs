using Microsoft.AspNetCore.Mvc;
using Common.Models;
using System.Security.Claims;

namespace ServiceLayer.Service.Interfaces
{
    public interface IArticleService
    {
        Task<ActionResult<ArticlesDto>> CreateNewArticleAsync(ArticleCreateDto articleDto, ClaimsPrincipal user);
        Task<ActionResult<ArticlesDto>> EditArticleAsync(ArticleEditDto articleEditDto);
        Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id);
        Task<ActionResult<List<ArticlesDto>>> GetAllArticlesAsync();
    }
}
