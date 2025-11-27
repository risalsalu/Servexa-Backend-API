namespace Servexa.Application.DTOs.Auth.Common
{
    public class SocialLoginDto
    {
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";
        public string Provider { get; set; } = default!;  // Google, Facebook
        public string ProviderUserId { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
    }
}
