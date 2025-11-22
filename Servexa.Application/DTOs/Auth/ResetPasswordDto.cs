namespace Servexa.Application.DTOs.Auth;

public class ResetPasswordDto
{
    public string Token { get; set; } = default!;    // later: real reset token
    public string NewPassword { get; set; } = default!;
}
