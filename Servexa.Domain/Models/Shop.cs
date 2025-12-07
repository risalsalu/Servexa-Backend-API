using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("Shops")]
    public class Shop : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool HomeServiceAvailable { get; set; }
        public string? Services { get; set; }
        public string? WorkingHours { get; set; }
        public bool IsActive { get; set; }
    }
}
