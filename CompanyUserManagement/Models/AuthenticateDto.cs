using System.ComponentModel.DataAnnotations;

namespace CompanyUserManagement.Models
{
    public class AuthenticateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }
    }
}
