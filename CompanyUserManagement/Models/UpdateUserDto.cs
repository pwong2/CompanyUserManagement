using System.ComponentModel.DataAnnotations;

namespace CompanyUserManagement.Models
{
    public class UpdateUserDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number.")]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string? Username { get; set; } // Nullable to handle optional updates

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? Password { get; set; } // Nullable to handle optional updates

        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive number.")]
        public int? CompanyId { get; set; } // Nullable to handle optional updates

        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
        public string? Role { get; set; } // Nullable to handle optional updates
    }
}
