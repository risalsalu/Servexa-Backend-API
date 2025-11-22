namespace Servexa.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public int ExpiresIn { get; set; }           // seconds
    public string Role { get; set; } = default!;
    public Guid UserId { get; set; }
}
