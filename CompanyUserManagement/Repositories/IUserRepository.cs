using CompanyUserManagement.Models;

namespace CompanyUserManagement.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        IEnumerable<User> GetUsersByCompanyId(int companyId);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
        IEnumerable<User> GetAllUsers();
    }
}
