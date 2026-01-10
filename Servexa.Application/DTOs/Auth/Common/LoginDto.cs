namespace Servexa.Application.DTOs.Auth.Common
{
    public class LoginDto
    {
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
