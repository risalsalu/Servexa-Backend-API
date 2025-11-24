namespace Servexa.Application.DTOs.Auth.Common;

public class LoginDto
{
    public string EmailOrPhone { get; set; } = default!;
    public string Password { get; set; } = default!;
}
