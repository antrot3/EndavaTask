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

        public UsersController(TokenService tokenService, ILogger<UsersController> logger)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
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
            return BadRequest(ex.Message);
        }
    }
}