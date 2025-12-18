using System;

namespace Servexa.Application.DTOs.Shop
{
    public class AddShopImageDto
    {
        public Guid ShopId { get; set; }
        public string ImageBase64 { get; set; } = string.Empty;
        public int ImageType { get; set; }
    }
}
