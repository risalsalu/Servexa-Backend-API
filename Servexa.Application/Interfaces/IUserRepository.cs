using Servexa.Domain.Models;
using Servexa.Application.DTOs.Admin;

namespace Servexa.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailOrPhoneAsync(string value);
        Task<bool> EmailOrPhoneExistsAsync(string email, string phone);
        Task CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> SetActiveStatusAsync(Guid id, bool isActive);
        Task<bool> SoftDeleteAsync(Guid id, Guid deletedBy);
        Task<IEnumerable<AdminShopOwnerListDto>> GetAllShopOwnersWithShopStatusAsync();
    }
}
