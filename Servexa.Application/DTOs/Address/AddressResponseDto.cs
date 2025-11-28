using System;

namespace Servexa.Application.DTOs.Address
{
    public class AddressResponseDto
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = default!;
        public string Line1 { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Pincode { get; set; } = default!;
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
