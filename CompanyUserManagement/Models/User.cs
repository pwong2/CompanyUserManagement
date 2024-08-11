using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyUserManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // This should be managed securely

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive number.")]
        public int CompanyId { get; set; }

        [Required]
        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PasswordHash { get; set; }
    }
}
