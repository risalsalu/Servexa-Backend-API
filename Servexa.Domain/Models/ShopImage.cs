using System;

namespace Servexa.Domain.Models
{
    public class ShopImage : BaseEntity
    {
        public Guid ShopId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
    }
}
