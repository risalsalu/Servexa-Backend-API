namespace Servexa.Application.DTOs.Shop
{
    public class AddShopDto
    {
        public string ShopName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool HomeServiceAvailable { get; set; }
        public string Services { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
    }
}
