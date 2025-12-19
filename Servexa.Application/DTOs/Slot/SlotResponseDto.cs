using System;

namespace Servexa.Application.DTOs.Slot
{
    public class SlotResponseDto
    {
        public Guid SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
