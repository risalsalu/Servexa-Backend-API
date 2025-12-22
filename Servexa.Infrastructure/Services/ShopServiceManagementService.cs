using Servexa.Application.DTOs.Services;
using Servexa.Application.Interfaces;
using DomainShopService = Servexa.Domain.Models.ShopService;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class ShopServiceManagementService : IShopServiceManagementService
    {
        private readonly IShopServiceRepository _repo;

        public ShopServiceManagementService(IShopServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ShopServiceResponseDto> AddServiceAsync(Guid shopId, AddShopServiceDto dto)
        {
            var entity = new DomainShopService
            {
                Id = Guid.NewGuid(),
                ShopId = shopId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repo.AddAsync(entity);

            return new ShopServiceResponseDto
            {
                Id = entity.Id,
                ShopId = entity.ShopId,
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedOn,
                UpdatedAtUtc = entity.ModifiedOn
            };
        }

        public async Task<ShopServiceResponseDto> UpdateServiceAsync(Guid shopId, Guid serviceId, UpdateShopServiceDto dto)
        {
            var entity = await _repo.GetByIdAsync(serviceId);
            if (entity == null || entity.ShopId != shopId)
                throw new Exception("Not found");

            entity.CategoryId = dto.CategoryId;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.DurationMinutes = dto.DurationMinutes;
            entity.IsActive = dto.IsActive;
            entity.ModifiedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);

            return new ShopServiceResponseDto
            {
                Id = entity.Id,
                ShopId = entity.ShopId,
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedOn,
                UpdatedAtUtc = entity.ModifiedOn
            };
        }

        public async Task<bool> DeleteServiceAsync(Guid shopId, Guid serviceId, Guid deletedBy)
        {
            var entity = await _repo.GetByIdAsync(serviceId);
            if (entity == null || entity.ShopId != shopId)
                throw new Exception("Not found");

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedOn = DateTime.UtcNow;
            entity.ModifiedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            return true;
        }

        public async Task<IEnumerable<ShopServiceResponseDto>> GetServicesForOwnerAsync(Guid shopId)
        {
            var items = await _repo.GetByShopAsync(shopId);

            return items.Select(s => new ShopServiceResponseDto
            {
                Id = s.Id,
                ShopId = s.ShopId,
                CategoryId = s.CategoryId,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes,
                IsActive = s.IsActive,
                CreatedAtUtc = s.CreatedOn,
                UpdatedAtUtc = s.ModifiedOn
            });
        }

        public async Task<IEnumerable<ShopServiceListItemDto>> GetServicesForUserAsync(Guid shopId)
        {
            var items = await _repo.GetActiveByShopAsync(shopId);

            return items.Select(s => new ShopServiceListItemDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes
            });
        }
    }
}
