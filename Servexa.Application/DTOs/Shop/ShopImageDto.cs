using System;

namespace Servexa.Application.DTOs.Shop
{
    public class ShopImageDto
    {
        public Guid ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int ImageType { get; set; }
        public string ImageTypeName { get; set; } = string.Empty;
    }
}
