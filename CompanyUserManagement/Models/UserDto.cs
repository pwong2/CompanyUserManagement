namespace CompanyUserManagement.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }

        public int CompanyId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
