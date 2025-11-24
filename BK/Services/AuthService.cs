using System.Security.Cryptography;
using System.Text;
using BK.Models;
using BK.Models.DTO;
using BK.Repositories;

namespace BK.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public AuthResponseDTO Login(LoginRequestDTO loginDto)
        {
            var user = _userRepository.GetUserByLoginOrEmail(loginDto.LoginOrEmail);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверные учетные данные");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Аккаунт деактивирован");
            }

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(7);

            _userRepository.SaveRefreshToken(user.Id, refreshToken, expiry);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = expiry,
                User = new UserResponseDTO
                {
                    Id = user.Id,
                    Login = user.Login,
                    Email = user.Email,
                    Role = user.Role.Name,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public AuthResponseDTO Register(RegisterRequestDTO registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Login) || string.IsNullOrWhiteSpace(registerDto.Email))
            {
                throw new ArgumentException("Логин и email обязательны для заполнения");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
            {
                throw new ArgumentException("Пароль должен содержать минимум 6 символов");
            }

            var existingUser = _userRepository.GetUserByLoginOrEmail(registerDto.Login);
            if (existingUser != null)
            {
                throw new ArgumentException("Пользователь с таким логином уже существует");
            }

            existingUser = _userRepository.GetUserByLoginOrEmail(registerDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("Пользователь с таким email уже существует");
            }

            var userRole = _userRepository.GetRoleByName(UserRoles.User);
            if (userRole == null)
            {
                throw new ArgumentException("Роль пользователя не найдена");
            }

            var user = new User
            {
                Login = registerDto.Login.Trim(),
                Email = registerDto.Email.Trim().ToLower(),
                PasswordHash = HashPassword(registerDto.Password),
                RoleId = userRole.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = _userRepository.AddUser(user);
            var token = _jwtService.GenerateToken(createdUser);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(7);

            _userRepository.SaveRefreshToken(createdUser.Id, refreshToken, expiry);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = expiry,
                User = new UserResponseDTO
                {
                    Id = createdUser.Id,
                    Login = createdUser.Login,
                    Email = createdUser.Email,
                    Role = createdUser.Role.Name,
                    CreatedAt = createdUser.CreatedAt
                }
            };
        }

        public AuthResponseDTO RefreshToken(RefreshTokenRequestDTO refreshRequest)
        {
            if (!_jwtService.ValidateToken(refreshRequest.Token))
            {
                throw new UnauthorizedAccessException("Недействительный токен");
            }

            var user = _userRepository.GetUserByRefreshToken(refreshRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Недействительный refresh token");
            }

            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(7);

            _userRepository.SaveRefreshToken(user.Id, newRefreshToken, expiry);

            return new AuthResponseDTO
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                Expiration = expiry,
                User = new UserResponseDTO
                {
                    Id = user.Id,
                    Login = user.Login,
                    Email = user.Email,
                    Role = user.Role.Name,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public bool ChangePassword(int userId, ChangePasswordDTO changePasswordDto)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new ArgumentException("Пользователь не найден");
            }

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Текущий пароль неверен");
            }

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.UpdateAt = DateTime.UtcNow;
            _userRepository.UpdateUser(user);

            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
    }
}