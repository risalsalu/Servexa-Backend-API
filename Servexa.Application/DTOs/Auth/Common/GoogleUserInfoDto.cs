namespace Servexa.Application.DTOs.Auth.Common
{
    public class GoogleUserInfoDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
    }
}
