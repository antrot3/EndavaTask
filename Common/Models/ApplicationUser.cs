using Microsoft.AspNetCore.Identity;
using Common.Enums;

namespace Common.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; }
    }
}
