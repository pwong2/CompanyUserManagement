using System.ComponentModel.DataAnnotations;

namespace CompanyUserManagement.Models
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive number.")]
        public int CompanyId { get; set; }

        [Required]
        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
        public string Role { get; set; }
    }
}
