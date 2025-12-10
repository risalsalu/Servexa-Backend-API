using System;

namespace Servexa.Application.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        public int? Quantity { get; set; }
        public DateTime? SelectedDateTime { get; set; }
    }
}
