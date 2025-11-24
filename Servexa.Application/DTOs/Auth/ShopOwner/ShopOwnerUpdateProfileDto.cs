namespace Servexa.Application.DTOs.Auth.ShopOwner
{
    public class ShopOwnerUpdateProfileDto
    {
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string ShopName { get; set; } = null!;
        public string ShopAddress { get; set; } = null!;
        public string ShopDescription { get; set; } = null!;
    }
}
