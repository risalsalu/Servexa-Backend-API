namespace Servexa.Application.DTOs.Auth;

public class SocialLoginDto
{
    public string Provider { get; set; } = default!;     // "Google"
    public string AccessToken { get; set; } = default!;
}
