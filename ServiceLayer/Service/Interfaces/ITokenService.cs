using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Common.Models;

namespace ServiceLayer.Service.Interfaces
{
    public interface ITokenService
    {
        Task<ActionResult<AuthResponse>> Authenticate(AuthRequest request);
        Task<IdentityResult> Register(RegistrationRequest request);
    }
}
