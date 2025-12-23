using System.Collections.Generic;

namespace Servexa.Application.DTOs.Booking
{
    public class ShopBookingsResponseDto
    {
        public decimal TotalRevenue { get; set; }
        public IEnumerable<ShopBookingDetailDto> Bookings { get; set; } = [];
    }
}
