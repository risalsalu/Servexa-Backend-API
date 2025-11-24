namespace Servexa.Application.DTOs.Auth.Common;

public class ResetPasswordDto
{
    public string Token { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
