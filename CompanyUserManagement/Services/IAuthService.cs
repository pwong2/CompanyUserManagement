using CompanyUserManagement.Models;

namespace CompanyUserManagement.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        User AuthenticateUser(string username, string password); // Method signature
    }
}
