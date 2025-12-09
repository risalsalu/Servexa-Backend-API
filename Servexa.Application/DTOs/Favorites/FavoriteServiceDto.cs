using System;

namespace Servexa.Application.DTOs.Favorites
{
    public class FavoriteServiceDto
    {
        public Guid Id { get; set; }
        public Guid ShopServiceId { get; set; }
        public string ServiceName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = "";
    }
}
