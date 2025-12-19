using System;

namespace Servexa.Domain.Models
{
    public class Booking : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ShopId { get; set; }
        public Guid? AddressId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public ServiceMode ServiceMode { get; set; }
    }
}
