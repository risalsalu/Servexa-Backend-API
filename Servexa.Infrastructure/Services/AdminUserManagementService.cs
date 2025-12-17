using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using Servexa.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class AdminUserManagementService : IAdminUserManagementService
    {
        private readonly IUserRepository _repo;

        public AdminUserManagementService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<IEnumerable<AdminUserListDto>>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllUsersAsync();

            var dto = users
                .Where(u => u.Role == "Customer")
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive
                });

            return ApiResponse<IEnumerable<AdminUserListDto>>.SuccessResponse(dto, "Customers fetched successfully");
        }

        public async Task<ApiResponse<IEnumerable<AdminShopOwnerListDto>>> GetAllShopOwnersAsync()
        {
            var owners = await _repo.GetAllShopOwnersWithShopStatusAsync();

            var dto = owners.Select(o => new AdminShopOwnerListDto
            {
                Id = o.Id,
                FullName = o.FullName,
                Email = o.Email,
                Phone = o.Phone,
                IsActive = o.IsActive,
                ShopId = o.ShopId,
                ShopName = o.ShopName,
                ShopIsActive = o.ShopIsActive,
                ShopOfflineReason = o.ShopOfflineReason
            });

            return ApiResponse<IEnumerable<AdminShopOwnerListDto>>.SuccessResponse(dto, "Shop owners fetched successfully");
        }

        public async Task<ApiResponse<bool>> SetUserActiveStatusAsync(Guid id, bool isActive)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<bool>.ErrorResponse("User not found");

            if (user.Role != "Customer")
                return ApiResponse<bool>.ErrorResponse("The user is not a customer");

            var updated = await _repo.SetActiveStatusAsync(id, isActive);
            if (!updated)
                return ApiResponse<bool>.ErrorResponse("Failed to update customer status");

            return ApiResponse<bool>.SuccessResponse(true, "Customer status updated");
        }

        public async Task<ApiResponse<bool>> SetShopOwnerActiveStatusAsync(Guid id, bool isActive)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<bool>.ErrorResponse("User not found");

            if (user.Role != "ShopOwner")
                return ApiResponse<bool>.ErrorResponse("The user is not a shop owner");

            var updated = await _repo.SetActiveStatusAsync(id, isActive);
            if (!updated)
                return ApiResponse<bool>.ErrorResponse("Failed to update shop owner status");

            return ApiResponse<bool>.SuccessResponse(true, "Shop owner status updated");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id, Guid adminId)
        {
            var deleted = await _repo.SoftDeleteAsync(id, adminId);
            if (!deleted)
                return ApiResponse<bool>.ErrorResponse("Failed to delete user");

            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }
    }
}
