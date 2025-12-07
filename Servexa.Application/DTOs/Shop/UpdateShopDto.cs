using System;

namespace Servexa.Application.DTOs.Shop
{
    public class UpdateShopDto
    {
        public string ShopName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool HomeServiceAvailable { get; set; }
        public WorkingHoursDto WorkingHours { get; set; } = new();
    }
}
