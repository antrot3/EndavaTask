using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ServiceLayer.Service.Interfaces
{
    public interface IDatabaseService
    {
        Task<ApplicationUser?> GetCurrentUserAsync(string? userName);
        Task AddArticleAndSaveChangesAsync(Article article);
        Task<Article?> GetArticleByIdAsync(int? articleId); 
        Task<Article> GetArticleByFindIdAsync(int id);
        Task<ApplicationUser?> GetUserByIdAsync(string? userId);
        Task<List<Article>> GetAllArticlesAsync();
        Task<Article> DeleteArticleByIdAsync(int articleId);
        Task SaveChangesAsync();
    }
}