using System;

namespace Servexa.Domain.Models
{
    public class ShopService : BaseEntity
    {
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
