using Common.Models;
using DAL;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Service.Interfaces;

namespace ServiceLayer.Service
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(string? userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task AddArticleAndSaveChangesAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int? articleId)
        {
            return await _context.Articles.FirstOrDefaultAsync(x => x.Id == articleId);
        }

        public async Task<Article> GetArticleByFindIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string? userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}