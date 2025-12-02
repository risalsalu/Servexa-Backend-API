namespace Servexa.Application.DTOs.Address
{
    public class UpdateAddressDto
    {
        public string Label { get; set; } = string.Empty;
        public string Line1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
