namespace Servexa.Domain.Models
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool IsActive { get; set; }

        public string? BusinessName { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? ShopPhotoUrl { get; set; }
        public string? LicenseDocumentUrl { get; set; }
        public string? IdCardUrl { get; set; }
    }
}
