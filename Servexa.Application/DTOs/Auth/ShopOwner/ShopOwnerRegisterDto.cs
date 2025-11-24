namespace Servexa.Application.DTOs.Auth.ShopOwner;

public class ShopOwnerRegisterDto
{
    public string OwnerName { get; set; } = default!;
    public string BusinessName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = "ShopOwner";

    public string Address { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string ShopPhotoUrl { get; set; } = default!;
    public string LicenseDocumentUrl { get; set; } = default!;
    public string IdCardUrl { get; set; } = default!;
}
