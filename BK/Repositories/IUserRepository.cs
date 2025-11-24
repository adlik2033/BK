using BK.Models;

namespace BK.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        User GetUserByLoginOrEmail(string loginOrEmail);
        User AddUser(User user);
        bool DeleteUser(int id);
        User UpdateUser(User user);
        bool SaveRefreshToken(int userId, string refreshToken, DateTime expiry);
        User GetUserByRefreshToken(string refreshToken);
        IEnumerable<User> GetAllUsers();
        Role GetRoleById(int id);
        Role GetRoleByName(string roleName);
        IEnumerable<Role> GetAllRoles();
        Role CreateRole(Role role);
        bool DeleteRole(int id);
    }
}