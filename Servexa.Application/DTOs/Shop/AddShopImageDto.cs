using System;

namespace Servexa.Application.DTOs.Shop
{
    public class AddShopImageDto
    {
        public Guid ImageId { get; set; }
        public string ImageUrl { get; set; } = default!;
    }
}
