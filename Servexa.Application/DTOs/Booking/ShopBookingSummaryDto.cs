using System;

namespace Servexa.Application.DTOs.Booking
{
    public class ShopBookingSummaryDto
    {
        public int TotalBookings { get; set; }
        public int TotalServicesBooked { get; set; }
        public decimal TotalRevenue { get; set; }
        public long ExecutionTimeMs { get; set; }
    }
}
