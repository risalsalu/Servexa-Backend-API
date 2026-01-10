using System;

namespace Servexa.Application.DTOs.Auth.Common
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
