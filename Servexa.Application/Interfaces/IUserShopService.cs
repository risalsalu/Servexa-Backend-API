using Servexa.Application.DTOs.UserServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IUserShopService
    {
        Task<IEnumerable<UserShopListDto>> GetShopsAsync();
        Task<UserShopWithServicesDto?> GetShopServicesAsync(Guid shopId);
    }
}
