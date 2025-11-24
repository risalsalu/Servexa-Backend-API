namespace Servexa.Application.DTOs.Auth.Common;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
    public string Role { get; set; } = default!;
    public Guid UserId { get; set; }
}
