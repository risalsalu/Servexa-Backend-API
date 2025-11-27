using Microsoft.AspNetCore.Http;

namespace Servexa.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
