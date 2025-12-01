using Microsoft.AspNetCore.Http;

namespace Servexa.API.Models;

public class AddShopRequest
{
    public string ShopName { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool HomeServiceAvailable { get; set; }
    public string Services { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = string.Empty;

    public IFormFile ShopImage { get; set; } = null!;
    public IFormFile LicenseImage { get; set; } = null!;
    public IFormFile IdProofImage { get; set; } = null!;
}
