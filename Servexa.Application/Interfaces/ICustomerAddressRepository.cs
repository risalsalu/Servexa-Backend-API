using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ICustomerAddressRepository
    {
        Task<IEnumerable<CustomerAddress>> GetByUserIdAsync(Guid userId);
        Task<CustomerAddress?> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(CustomerAddress address);
        Task<bool> UpdateAsync(CustomerAddress address);
        Task<bool> DeleteAsync(Guid id, Guid deletedBy);
    }
}
