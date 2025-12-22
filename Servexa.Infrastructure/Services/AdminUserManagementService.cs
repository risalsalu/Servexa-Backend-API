using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class AdminUserManagementService : IAdminUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;

        public AdminUserManagementService(
            IUserRepository userRepository,
            IBookingRepository bookingRepository)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<AdminUserListDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users
                .Where(u => u.Role == "Customer")
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive
                });
        }

        public async Task<IEnumerable<AdminShopOwnerListDto>> GetAllShopOwnersAsync()
        {
            var owners = await _userRepository.GetAllShopOwnersWithShopStatusAsync();

            return owners.Select(o => new AdminShopOwnerListDto
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
        }

        public async Task<bool> SetUserActiveStatusAsync(Guid id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            if (user.Role != "Customer")
                throw new Exception("The user is not a customer");

            if (!isActive)
            {
                var hasActiveBookings = await _bookingRepository.HasActiveBookingsAsync(id);
                if (hasActiveBookings)
                    throw new Exception("Customer has active bookings and cannot be blocked");
            }

            var updated = await _userRepository.SetActiveStatusAsync(id, isActive);
            if (!updated)
                throw new Exception("Failed to update customer status");

            return true;
        }

        public async Task<bool> SetShopOwnerActiveStatusAsync(Guid id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            if (user.Role != "ShopOwner")
                throw new Exception("The user is not a shop owner");

            var updated = await _userRepository.SetActiveStatusAsync(id, isActive);
            if (!updated)
                throw new Exception("Failed to update shop owner status");

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id, Guid adminId)
        {
            var deleted = await _userRepository.SoftDeleteAsync(id, adminId);
            if (!deleted)
                throw new Exception("Failed to delete user");

            return true;
        }
    }
}
