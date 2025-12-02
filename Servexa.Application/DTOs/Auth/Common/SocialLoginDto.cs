namespace Servexa.Application.DTOs.Auth.Common
{
    public class SocialLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
