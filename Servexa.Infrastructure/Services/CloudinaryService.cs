using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Servexa.Infrastructure.Settings;
    using Servexa.Application.Interfaces;

namespace Servexa.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "servexa_shopowners"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult?.SecureUrl?.ToString() ?? "";
        }
    }
}
