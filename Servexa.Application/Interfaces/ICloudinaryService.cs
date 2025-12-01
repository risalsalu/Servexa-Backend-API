using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces;

public interface ICloudinaryService
{
    Task<(string Url, string PublicId)> UploadAsync(IFormFile file);
    Task<bool> DeleteAsync(string publicId);
}
