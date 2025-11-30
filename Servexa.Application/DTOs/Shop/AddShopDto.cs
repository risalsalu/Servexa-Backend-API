namespace Servexa.Application.DTOs.Shop;

public class AddShopDto
{
    public string ShopName { get; set; } = default!;
    public string? Categories { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = default!;
    public bool HomeServiceAvailable { get; set; }
    public string? LicenseImageUrl { get; set; }
    public string? IdProofImageUrl { get; set; }
    public string? Services { get; set; }
    public string? WorkingHours { get; set; }
}
