namespace Servexa.Application.DTOs.Auth;

public class UpdateProfileDto
{
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? ProfileImage { get; set; }   // for future
    public string? Gender { get; set; }
}
