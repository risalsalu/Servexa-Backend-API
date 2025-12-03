using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using Servexa.Shared.Responses;
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

            return ApiResponse<IEnumerable<AdminUserListDto>>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<IEnumerable<AdminShopOwnerListDto>>> GetAllShopOwnersAsync()
        {
            var owners = await _repo.GetAllShopOwnersWithShopStatusAsync();

            return ApiResponse<IEnumerable<AdminShopOwnerListDto>>.SuccessResponse(owners);
        }

        public async Task<ApiResponse<bool>> SetUserActiveStatusAsync(Guid id, bool isActive)
        {
            var updated = await _repo.SetActiveStatusAsync(id, isActive);
            if (!updated)
                return ApiResponse<bool>.ErrorResponse("Failed.");

            return ApiResponse<bool>.SuccessResponse(true, "Updated.");
        }

        public async Task<ApiResponse<bool>> SetShopOwnerActiveStatusAsync(Guid id, bool isActive)
        {
            var updated = await _repo.SetActiveStatusAsync(id, isActive);
            if (!updated)
                return ApiResponse<bool>.ErrorResponse("Failed.");

            return ApiResponse<bool>.SuccessResponse(true, "Updated.");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id, Guid adminId)
        {
            var deleted = await _repo.SoftDeleteAsync(id, adminId);
            if (!deleted)
                return ApiResponse<bool>.ErrorResponse("Failed.");

            return ApiResponse<bool>.SuccessResponse(true, "Deleted.");
        }
    }
}
