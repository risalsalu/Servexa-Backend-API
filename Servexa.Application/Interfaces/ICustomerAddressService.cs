using Servexa.Application.DTOs.Address;

namespace Servexa.Application.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(Guid userId);
        Task<Guid> AddAddressAsync(Guid userId, AddAddressDto dto);
        Task<bool> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressDto dto);
        Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);
    }
}
