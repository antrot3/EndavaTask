using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Service;

namespace Endava.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(TokenService tokenService, ILogger<UsersController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            _logger.LogInformation($"Registration started for:{request.Username}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _tokenService.Register(request);

                if (result.Succeeded)
                {
                    request.Password = string.Empty; // Ensuring password isn't returned
                    return CreatedAtAction(nameof(Register), new { email = request.Email, role = Role.User }, request);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                
                _logger.LogInformation($"Registration sucess");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            _logger.LogInformation($"Authenticate started");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _logger.LogInformation($"Authenticate sucess");
                return await _tokenService.Authenticate(request);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // Helper method to handle exceptions
        private ActionResult HandleException(Exception ex)
        {
            _logger.LogError($"Exception message:{ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}