using System;

namespace Servexa.Domain.Models
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ShopId { get; set; }
    }
}
