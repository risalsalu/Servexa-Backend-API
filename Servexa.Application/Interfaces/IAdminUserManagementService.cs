using Servexa.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IAdminUserManagementService
    {
        Task<IEnumerable<AdminUserListDto>> GetAllUsersAsync();
        Task<IEnumerable<AdminShopOwnerListDto>> GetAllShopOwnersAsync();
        Task<bool> SetUserActiveStatusAsync(Guid id, bool isActive);
        Task<bool> SetShopOwnerActiveStatusAsync(Guid id, bool isActive);
        Task<bool> DeleteUserAsync(Guid id, Guid adminId);
    }
}
