using Common.Models;
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
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService articleService)
        {
            _articleService = articleService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateArticle")]
        public async Task<ActionResult<ArticlesDto>> CreateArticleAsync(ArticleCreateDto articleDto)
        {
            return await HandleRequestAsync(() => _articleService.CreateNewArticleAsync(articleDto, User), "CreateArticleAsync", articleDto.Title);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("EditArticle")]
        public async Task<ActionResult<ArticlesDto>> EditArticleAsync(ArticleEditDto articleEditDto)
        {
            return await HandleRequestAsync(() => _articleService.EditArticleAsync(articleEditDto), "EditArticleAsync", articleEditDto.Title);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ArticlesDto>> GetArticleByIdAsync(int id)
        {
            return await HandleRequestAsync(() => _articleService.GetArticleByIdAsync(id), "GetArticleByIdAsync", id.ToString());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("DeleteArticle/{id:int}")]
        public async Task<ActionResult<ArticlesDto>> DeleteArticle(int id)
        {
            return await HandleRequestAsync(() => _articleService.DeleteArticleByIdAsync(id), "GetArticleByIdAsync", id.ToString());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetAllArticlesAsync")]
        public async Task<ActionResult<List<ArticlesDto>>> GetAllArticlesAsync()
        {
            return await HandleRequestAsync(() => _articleService.GetAllArticlesAsync(), "GetAllArticlesAsync", "");
        }

        private async Task<ActionResult<T>> HandleRequestAsync<T>(Func<Task<ActionResult<T>>> serviceCall, string methodName, string identifier = "")
        {
            _logger.LogInformation($"{methodName} started {(!string.IsNullOrEmpty(identifier) ? $"for: {identifier}" : "")}");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state not valid");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await serviceCall();
                _logger.LogInformation($"{methodName} finished {(!string.IsNullOrEmpty(identifier) ? $"for: {identifier}" : "")}");
                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private ActionResult HandleException(Exception ex)
        {
            _logger.LogError($"Exception thrown: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}