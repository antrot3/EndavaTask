using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Models
{
    public class RegistrationRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public bool UserIsAdmin { get; set; }
    }
}
