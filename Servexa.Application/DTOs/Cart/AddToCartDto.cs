using System;

namespace Servexa.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        public Guid ShopId { get; set; }
        public Guid ShopServiceId { get; set; }
        public int Quantity { get; set; }
        public DateTime SelectedDateTime { get; set; }
    }
}
