using System;

namespace Servexa.Application.DTOs.UserServices
{
    public class UserShopListDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
