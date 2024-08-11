using CompanyUserManagement.Models;
using CompanyUserManagement.Repositories;
using CompanyUserManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CompanyUserManagement.Exceptions.Exceptions;

namespace CompanyUserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, IAuthService authService, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateDto authenticateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _authService.AuthenticateUser(authenticateDto.Username, authenticateDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        [Authorize]
        public IActionResult Register(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid user data. Please check the input and try again."); // Throw BadRequestException
            }

            // Check if the logged-in user is an Admin
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedException("Only Admin users can register new users."); // Throw UnauthorizedException
            }

            try {
                var user = new User
                {
                    Username = createUserDto.Username,
                    Password = createUserDto.Password,
                    Role = createUserDto.Role,
                    CompanyId = createUserDto.CompanyId
                };

                _userRepository.CreateUser(user);

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                // Let the middleware handle the exception
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid data provided."); // Throw custom exception
            }

            // Check if the logged-in user is an Admin
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedException("Only Admin users can update users."); // Throw custom exception
            }

            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new NotFoundException("User not found."); // Throw custom exception
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateUserDto.Username))
            {
                user.Username = updateUserDto.Username;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            if (updateUserDto.CompanyId.HasValue)
            {
                user.CompanyId = updateUserDto.CompanyId.Value;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                user.Role = updateUserDto.Role;
            }

            try
            {
                _userRepository.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("An unexpected error occurred while updating the user."); // Throw custom exception
            }

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            // Validate the request data
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid data provided.");
            }

            // Check if the logged-in user is an Admin
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedException("Only Admin users can delete users.");
            }

            // Retrieve the user from the repository
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Delete the user
            try
            {
                _userRepository.DeleteUser(id);
            }
            catch (Exception ex)
            {
                // Log the exception if needed and throw an internal server error
                _logger.LogError(ex, "An error occurred while deleting the user.");
                throw new InternalServerErrorException("An unexpected error occurred while deleting the user.");
            }

            // Return a success message
            return Ok(new { message = "User deleted successfully" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            // Retrieve the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            // Validate the user ID claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in claims." });
            }

            // Retrieve the current user from the repository
            var currentUser = _userRepository.GetUserById(userId);

            if (currentUser == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            // Retrieve all users from the current user's company
            var users = _userRepository.GetUsersByCompanyId(currentUser.CompanyId);

            // If the current user is a flat user, filter out admin users from the list
            if (currentUser.Role != "Admin")
            {
                users = users.Where(u => u.Role != "Admin").ToList();
            }

            // Map the users to a DTO for returning to the client
            var usersDto = users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                CompanyId = user.CompanyId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            // Return the filtered list of users
            return Ok(usersDto);
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            // Check if the current user has the "Admin" role
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin")
            {
                // Return Unauthorized response with a custom message if not an Admin
                return Unauthorized(new { message = "You do not have the required permissions to access this resource." });
            }

            try
            {
                // Retrieve all users from the repository, regardless of company
                var users = _userRepository.GetAllUsers(); // Ensure this method is implemented in your repository

                // Map the users to a DTO for returning to the client
                var usersDto = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    CompanyId = user.CompanyId,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }).ToList();

                // Return the list of all users
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error response
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

    }
}
