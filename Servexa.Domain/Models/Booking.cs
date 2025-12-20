using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("Bookings")]
    public class Booking : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid ShopId { get; set; }
        public Guid PaymentId { get; set; }
        public ServiceMode ServiceMode { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
        public decimal Amount { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
