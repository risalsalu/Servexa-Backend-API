using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Servexa.Application.Interfaces;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<(string Url, string PublicId)> UploadAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var upload = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "servexa/shops"
        };

        var result = await _cloudinary.UploadAsync(upload);
        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<bool> DeleteAsync(string publicId)
    {
        var deletion = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletion);
        return result.Result == "ok";
    }
}
