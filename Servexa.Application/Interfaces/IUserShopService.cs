using Servexa.Application.DTOs.UserServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IUserShopService
    {
        Task<IEnumerable<UserShopListDto>> GetShopsAsync(Guid customerId);
        Task<UserShopWithServicesDto?> GetShopServicesAsync(Guid customerId, Guid shopId);
    }
}
