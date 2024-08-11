using CompanyUserManagement.Models;
using CompanyUserManagement.Repositories;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using static CompanyUserManagement.Exceptions.Exceptions;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
    {
        _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _logger = logger;
    }

    public User GetUserById(int id)
    {
        try
        {
            return _dbConnection.QuerySingleOrDefault<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user by ID");
            throw;
        }
    }

    public User GetUserByUsername(string username)
    {
        try
        {
            return _dbConnection.QuerySingleOrDefault<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user by username");
            throw;
        }
    }

    public IEnumerable<User> GetUsersByCompanyId(int companyId)
    {
        try
        {
            return _dbConnection.Query<User>("SELECT * FROM Users WHERE CompanyId = @CompanyId", new { CompanyId = companyId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users by company ID");
            throw;
        }
    }

    public void CreateUser(User user)
    {
        try
        {
            // Validate the user input
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(user.Password));
            }

            if (user.CompanyId <= 0)
            {
                throw new ArgumentException("Invalid CompanyId", nameof(user.CompanyId));
            }

            // Hash the password before saving
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = null; // Clear the plain password for security reasons

            // Check if the user already exists for the company
            var existingUser = _dbConnection.QuerySingleOrDefault<User>(
                "SELECT * FROM Users WHERE Username = @Username AND CompanyId = @CompanyId",
                new { user.Username, user.CompanyId }
            );

            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with the same username already exists in this company.");
            }

            // Insert the new user into the database
            _dbConnection.Execute(
                "INSERT INTO Users (Username, PasswordHash, Role, CompanyId) VALUES (@Username, @PasswordHash, @Role, @CompanyId)",
                user
            );
        }
        catch (ArgumentException argEx)
        {
            _logger.LogError(argEx, "Invalid argument error creating user");
            // Handle ArgumentException specifically
            throw new BadRequestException(argEx.Message); // Custom exception for bad requests
        }
        catch (InvalidOperationException invOpEx)
        {
            _logger.LogError(invOpEx, "Invalid operation error creating user");
            // Handle InvalidOperationException specifically
            throw new ConflictException(invOpEx.Message); // Custom exception for conflicts
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user");
            // Handle other exceptions
            throw new InternalServerErrorException("An unexpected error occurred. Please try again later."); // Custom exception for internal server errors
        }
    }

    public void UpdateUser(User user)
    {
        try
        {
            // Validate the user input
            if (user.Id <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(user.Id));
            }

            if (user.CompanyId <= 0)
            {
                throw new ArgumentException("Invalid CompanyId", nameof(user.CompanyId));
            }

            // Update the user in the database
            _dbConnection.Execute(
                "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash, Role = @Role, CompanyId = @CompanyId WHERE Id = @Id",
                user
            );
        }
        catch (ArgumentException argEx)
        {
            _logger.LogError(argEx, "Invalid argument error updating user");
            throw new BadRequestException(argEx.Message); // Custom exception for bad requests
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user");
            throw new InternalServerErrorException("An unexpected error occurred. Please try again later."); // Custom exception for internal server errors
        }
    }

    public void DeleteUser(int userId)
    {
        try
        {
            // Execute the deletion query
            int rowsAffected = _dbConnection.Execute("DELETE FROM Users WHERE Id = @UserId", new { UserId = userId });

            // Check if the user was deleted (if rowsAffected is 0, the user did not exist)
            if (rowsAffected == 0)
            {
                throw new NotFoundException("User not found.");
            }
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Database error occurred while deleting user.");
            throw new InternalServerErrorException("A database error occurred while deleting the user.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting the user.");
            throw new InternalServerErrorException("An unexpected error occurred. Please try again later.");
        }
    }

    public IEnumerable<User> GetAllUsers()
    {
        try
        {
            // Execute the query to retrieve all users
            return _dbConnection.Query<User>("SELECT * FROM Users");
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An error occurred while retrieving all users from the database.");
            return Enumerable.Empty<User>();
        }
    }

}