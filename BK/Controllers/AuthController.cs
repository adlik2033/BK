using BK.Models;
using BK.Models.DTO;
using BK.Services;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Ошибка валидации",
                        Details = "Проверьте правильность введенных данных"
                    });
                }

                var result = _authService.Login(loginDto);
                return Ok(new ApiResponse<AuthResponseDTO>
                {
                    Success = true,
                    Message = "Успешный вход в систему",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "Ошибка авторизации", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка входа", Details = ex.Message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Ошибка валидации",
                        Details = "Проверьте правильность введенных данных"
                    });
                }

                var result = _authService.Register(registerDto);
                return Ok(new ApiResponse<AuthResponseDTO>
                {
                    Success = true,
                    Message = "Пользователь успешно зарегистрирован",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка регистрации", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка регистрации", Details = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequestDTO refreshRequest)
        {
            try
            {
                var result = _authService.RefreshToken(refreshRequest);
                return Ok(new ApiResponse<AuthResponseDTO>
                {
                    Success = true,
                    Message = "Токен успешно обновлен",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "Ошибка обновления токена", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка обновления токена", Details = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var result = _authService.ChangePassword(userId, changePasswordDto);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Пароль успешно изменен",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "Ошибка смены пароля", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = "Ошибка смены пароля", Details = ex.Message });
            }
        }
    }
}