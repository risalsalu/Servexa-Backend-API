namespace Servexa.Application.DTOs.Auth;

public class LoginDto
{
    public string EmailOrPhone { get; set; } = default!;
    public string Password { get; set; } = default!;
}
