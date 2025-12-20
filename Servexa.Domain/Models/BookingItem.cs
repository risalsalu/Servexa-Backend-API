using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("BookingItems")]
    public class BookingItem : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid ServiceId { get; set; }
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
