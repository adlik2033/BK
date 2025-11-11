namespace BK.Models.DTO
{
#pragma warning disable
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ValidTo { get; set; }
        public UserDTO User { get; set; } = new UserDTO();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}
#pragma warning enable
