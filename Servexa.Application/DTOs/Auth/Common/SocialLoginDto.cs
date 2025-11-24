namespace Servexa.Application.DTOs.Auth.Common;

public class SocialLoginDto
{
    public string Provider { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
}
