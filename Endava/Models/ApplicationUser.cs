using Microsoft.AspNetCore.Identity;
using Endava.Enums;
using Microsoft.AspNetCore.Identity;
using Endava.Enums;

namespace Endava.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; }
    }
}
