using Servexa.Application.DTOs.Address;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ICustomerAddressRepository _repo;

        public CustomerAddressService(ICustomerAddressRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(Guid userId)
        {
            var addresses = await _repo.GetByUserIdAsync(userId);

            return addresses.Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Label = a.Label,
                Line1 = a.Line1,
                City = a.City,
                Pincode = a.Pincode,
                Lat = a.Lat,
                Lng = a.Lng
            });
        }

        public async Task<Guid> AddAddressAsync(Guid userId, AddAddressDto dto)
        {
            var address = new CustomerAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Label = dto.Label,
                Line1 = dto.Line1,
                City = dto.City,
                Pincode = dto.Pincode,
                Lat = dto.Lat,
                Lng = dto.Lng,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _repo.AddAsync(address);
        }

        public async Task<bool> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressDto dto)
        {
            var existing = await _repo.GetByIdAsync(addressId);
            if (existing == null || existing.UserId != userId || existing.IsDeleted)
                return false;

            existing.Label = dto.Label;
            existing.Line1 = dto.Line1;
            existing.City = dto.City;
            existing.Pincode = dto.Pincode;
            existing.Lat = dto.Lat;
            existing.Lng = dto.Lng;
            existing.ModifiedBy = userId;
            existing.ModifiedOn = DateTime.UtcNow;

            return await _repo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
        {
            var existing = await _repo.GetByIdAsync(addressId);
            if (existing == null || existing.UserId != userId || existing.IsDeleted)
                return false;

            return await _repo.DeleteAsync(addressId, userId);
        }
    }
}
