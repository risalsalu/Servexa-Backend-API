namespace Servexa.Application.DTOs.Auth.Common;

public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public int ExpiresIn { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
