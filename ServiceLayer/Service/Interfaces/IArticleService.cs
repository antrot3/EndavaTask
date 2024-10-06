using Microsoft.AspNetCore.Mvc;
using Common.Models;
using System.Security.Claims;

namespace ServiceLayer.Service.Interfaces
{
    public interface IArticleService
    {
        Task<ActionResult<ArticlesDto>> CreateNewArticleAsync(ArticleCreateDto artilceDto, ClaimsPrincipal user);
        ActionResult<ArticlesDto> EditArticle(ArticleEditDto articleEditDto);
        Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id);
        Task<List<ArticlesDto>> GetAllArticlesAsync();
    }
}
