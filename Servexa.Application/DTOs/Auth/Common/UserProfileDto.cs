namespace Servexa.Application.DTOs.Auth.Common;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string? ProfileImage { get; set; }
    public string? Gender { get; set; }
}
