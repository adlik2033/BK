using BK.Models;
using Microsoft.EntityFrameworkCore;

namespace BK.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BKDbContext _context;
        public UserRepository(BKDbContext context)
        {
            _context = context;
        }
        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public bool DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            else return false;

        }

        public User GetUserByLoginOrEmail(string loginOrEmail)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == loginOrEmail || u.Email == loginOrEmail);

            return user;
        }

        public User GetUserById(int id)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);
            if (user != null)
                return user;
            else return null;
        }

        public Role GetRoleById(int id)
        {
            return _context.Roles.FirstOrDefault(u => u.Id == id);
        }

        public Role GetRoleByName(string roleName)
        {
            return _context.Roles.FirstOrDefault(r => r.Name == roleName);
        }

        public User UpdateUser(User user)
        {
            var user1 = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (user1 != null)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return user;
        }

        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiry)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            user.UpdateAt = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.Include(u => u.Role).ToList();
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public Role CreateRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role;
        }

        public bool DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}