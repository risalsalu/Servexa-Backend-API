using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> EmailOrPhoneExistsAsync(string email, string phone);
    Task CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
}
