using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Cart
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public Guid ShopId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new List<CartItemResponseDto>();
        public decimal TotalPrice { get; set; }
        public int TotalDurationMinutes { get; set; }
    }
}
