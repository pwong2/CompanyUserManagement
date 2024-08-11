using CompanyUserManagement.Models;
using CompanyUserManagement.Repositories;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CompanyUserManagement.Exceptions.Exceptions;

namespace CompanyUserManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Retrieve the key from configuration
            var keyString = _configuration["Jwt:Secret"];

            // Check if the key is null or empty
            if (string.IsNullOrWhiteSpace(keyString))
            {
                throw new BadRequestException("JWT secret key is not configured.");
            }

            // Convert the key to bytes and ensure its length is sufficient
            var key = Encoding.ASCII.GetBytes(keyString);

            if (key.Length < 32) // 32 bytes = 256 bits
            {
                throw new BadRequestException("JWT secret key must be at least 256 bits long."); // Use BadRequestException for key length issues
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public User AuthenticateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password."); // Use UnauthorizedException for authentication failure
            }

            // Use BCrypt to verify the password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid username or password."); // Use UnauthorizedException for authentication failure
            }

            return user;
        }
    }
}
