using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Shop
{
    public class ShopResponseDto
    {
        public Guid ShopId { get; set; }
        public Guid OwnerId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool HomeServiceAvailable { get; set; }
        public bool IsActive { get; set; }
        public string? WorkingHours { get; set; }
        public List<string> Images { get; set; } = new();
    }
}
