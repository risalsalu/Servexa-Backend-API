using Servexa.Application.DTOs.Admin;
using Servexa.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IAdminUserManagementService
    {
        Task<ApiResponse<IEnumerable<AdminUserListDto>>> GetAllUsersAsync();
        Task<ApiResponse<IEnumerable<AdminShopOwnerListDto>>> GetAllShopOwnersAsync();
        Task<ApiResponse<bool>> SetUserActiveStatusAsync(Guid id, bool isActive);
        Task<ApiResponse<bool>> SetShopOwnerActiveStatusAsync(Guid id, bool isActive);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid id, Guid adminId);
    }
}
