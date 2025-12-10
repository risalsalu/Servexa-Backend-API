using System;

namespace Servexa.Domain.Models
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public Guid ShopServiceId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime SelectedDateTime { get; set; }
    }
}
