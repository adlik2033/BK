using BK.Models;
using System.Security.Claims;

namespace BK.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}