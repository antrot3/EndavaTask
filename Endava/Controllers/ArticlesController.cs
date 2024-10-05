using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Data;
using ServiceLayer.Models;
using ServiceLayer.Service.Interfaces;

namespace Endava.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _dbContext;

        public ArticlesController(ILogger<ArticlesController> logger, ApplicationDbContext dbContext, IArticleService articleService)
        {
            _dbContext = dbContext;
            _articleService = articleService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("new")]
        public async Task<ActionResult<ArticlesDto>> CreateArticle(ArticleCreateDto artilceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                return await _articleService.CreateNewArticleAsync(artilceDto, User);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit")]
        public async Task<ActionResult<ArticlesDto>> EditArticle(ArticleEditDto articleEditDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                return _articleService.EditArticle(articleEditDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ArticlesDto>> GetArticleById(int id)
        {
            try
            {
                return await _articleService.GetArticleByIdAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<ArticlesDto>>> ListArticles()
        {
            try
            {
                return await _articleService.GetAllArticlesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
    }
}
