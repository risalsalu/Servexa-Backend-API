using Servexa.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface ICustomerAddressRepository
    {
        Task<IEnumerable<CustomerAddress>> GetByUserIdAsync(Guid userId);
        Task<CustomerAddress?> GetByIdAsync(Guid id);
        Task<CustomerAddress?> GetActiveAddressAsync(Guid userId);
        Task<Guid> AddAsync(CustomerAddress address);
        Task<bool> UpdateAsync(CustomerAddress address);
        Task<bool> DeleteAsync(Guid id, Guid deletedBy);
    }
}
