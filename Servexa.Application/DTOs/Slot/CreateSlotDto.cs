using System;

namespace Servexa.Application.DTOs.Slot
{
    public class CreateSlotDto
    {
        public Guid ShopId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
