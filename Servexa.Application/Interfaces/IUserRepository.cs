using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailOrPhoneAsync(string value);

    Task<bool> EmailOrPhoneExistsAsync(string email, string phone);

    Task CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
}
