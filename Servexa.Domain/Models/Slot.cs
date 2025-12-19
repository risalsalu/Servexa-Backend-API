using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("Slots")]
    public class Slot : BaseEntity
    {
        public Guid ShopId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; }
        public Guid? BookedBy { get; set; }
    }
}
