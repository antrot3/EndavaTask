using Common.Models;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("CreateArticle")]
        public async Task<ActionResult<ArticlesDto>> CreateArticle(ArticleCreateDto articleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return await _articleService.CreateNewArticleAsync(articleDto, User);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("EditArticle")]
        public ActionResult<ArticlesDto> EditArticle(ArticleEditDto articleEditDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return _articleService.EditArticle(articleEditDto);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
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
                return HandleException(ex);
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
                return HandleException(ex);
            }
        }
        private ActionResult HandleException(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}