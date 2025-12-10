using System;

namespace Servexa.Application.DTOs.Cart
{
    public class CartItemResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid ShopServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime SelectedDateTime { get; set; }
    }
}
