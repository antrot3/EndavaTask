using Microsoft.AspNetCore.Identity;
using ServiceLayer.Enums;

namespace ServiceLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; }
    }
}
