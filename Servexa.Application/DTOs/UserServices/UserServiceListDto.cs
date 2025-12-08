namespace Servexa.Application.DTOs.UserServices
{
    public class UserServiceListDto
    {
        public Guid ServiceId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }
}
