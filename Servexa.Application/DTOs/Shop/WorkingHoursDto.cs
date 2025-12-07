namespace Servexa.Application.DTOs.Shop
{
    public class WorkingHoursDto
    {
        public DayHoursDto Monday { get; set; } = new();
        public DayHoursDto Tuesday { get; set; } = new();
        public DayHoursDto Wednesday { get; set; } = new();
        public DayHoursDto Thursday { get; set; } = new();
        public DayHoursDto Friday { get; set; } = new();
        public DayHoursDto Saturday { get; set; } = new();
        public DayHoursDto Sunday { get; set; } = new();
    }
}
