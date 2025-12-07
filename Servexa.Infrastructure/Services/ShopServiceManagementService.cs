using Servexa.Application.DTOs.Services;
using Servexa.Application.Interfaces;
using Servexa.Shared.Responses;
using DomainShopService = Servexa.Domain.Models.ShopService;

namespace Servexa.Infrastructure.Services
{
    public class ShopServiceManagementService : IShopServiceManagementService
    {
        private readonly IShopServiceRepository _repo;

        public ShopServiceManagementService(IShopServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<ShopServiceResponseDto>> AddServiceAsync(Guid shopId, AddShopServiceDto dto)
        {
            var entity = new DomainShopService
            {
                ShopId = shopId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var id = await _repo.AddAsync(entity);
            entity.Id = id;

            var response = new ShopServiceResponseDto
            {
                Id = entity.Id,
                ShopId = entity.ShopId,
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };

            return ApiResponse<ShopServiceResponseDto>.SuccessResponse(response);
        }

        public async Task<ApiResponse<ShopServiceResponseDto>> UpdateServiceAsync(Guid shopId, Guid serviceId, UpdateShopServiceDto dto)
        {
            DomainShopService? entity = await _repo.GetByIdAsync(serviceId);
            if (entity == null || entity.ShopId != shopId)
                return ApiResponse<ShopServiceResponseDto>.ErrorResponse("Not found");

            entity.CategoryId = dto.CategoryId;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.DurationMinutes = dto.DurationMinutes;
            entity.IsActive = dto.IsActive;
            entity.ModifiedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);

            var response = new ShopServiceResponseDto
            {
                Id = entity.Id,
                ShopId = entity.ShopId,
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };

            return ApiResponse<ShopServiceResponseDto>.SuccessResponse(response);
        }

        public async Task<ApiResponse<bool>> DeleteServiceAsync(Guid shopId, Guid serviceId, Guid deletedBy)
        {
            DomainShopService? entity = await _repo.GetByIdAsync(serviceId);
            if (entity == null || entity.ShopId != shopId)
                return ApiResponse<bool>.ErrorResponse("Not found");

            var result = await _repo.DeleteAsync(serviceId, deletedBy);
            return ApiResponse<bool>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ShopServiceResponseDto>>> GetServicesForOwnerAsync(Guid shopId)
        {
            var items = await _repo.GetByShopAsync(shopId);

            var list = items.Select(s => new ShopServiceResponseDto
            {
                Id = s.Id,
                ShopId = s.ShopId,
                CategoryId = s.CategoryId,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes,
                IsActive = s.IsActive,
                CreatedAtUtc = s.CreatedAtUtc,
                UpdatedAtUtc = s.UpdatedAtUtc
            });

            return ApiResponse<IEnumerable<ShopServiceResponseDto>>.SuccessResponse(list);
        }

        public async Task<ApiResponse<IEnumerable<ShopServiceListItemDto>>> GetServicesForUserAsync(Guid shopId)
        {
            var items = await _repo.GetActiveByShopAsync(shopId);

            var list = items.Select(s => new ShopServiceListItemDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes
            });

            return ApiResponse<IEnumerable<ShopServiceListItemDto>>.SuccessResponse(list);
        }
    }
}
