namespace Servexa.Application.DTOs.UserServices
{
    public class UserShopWithServicesDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = default!;
        public List<UserServiceListDto> Services { get; set; } = new();
    }
}
