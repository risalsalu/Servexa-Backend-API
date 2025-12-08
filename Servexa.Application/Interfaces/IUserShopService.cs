using Servexa.Application.DTOs.UserServices;

namespace Servexa.Application.Interfaces
{
    public interface IUserShopService
    {
        Task<IEnumerable<UserShopListDto>> GetActiveShopsAsync();
        Task<UserShopWithServicesDto?> GetShopServicesAsync(Guid shopId);
    }
}
