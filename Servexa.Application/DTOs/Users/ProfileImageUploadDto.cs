using Microsoft.AspNetCore.Http;

namespace Servexa.Application.DTOs.Users
{
    public class ProfileImageUploadDto
    {
        public IFormFile File { get; set; } = default!;
    }
}
