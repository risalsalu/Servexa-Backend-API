using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Shared.Responses;

namespace Servexa.Infrastructure.Services
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ICategoryRepository _repo;

        public AdminCategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            var data = list.Select(x => new CategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name
            });

            return ApiResponse<IEnumerable<CategoryResponseDto>>.SuccessResponse(data, "Categories fetched successfully");
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto, Guid adminId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Name is required.");

            var entity = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                CreatedBy = adminId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repo.AddAsync(entity);

            return ApiResponse<CategoryResponseDto>.SuccessResponse(
                new CategoryResponseDto
                {
                    Id = entity.Id,
                    Name = entity.Name
                },
                "Category created successfully"
            );
        }

        public async Task<ApiResponse<CategoryResponseDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid adminId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Category not found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Name is required");

            existing.Name = dto.Name;
            existing.ModifiedBy = adminId;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);

            return ApiResponse<CategoryResponseDto>.SuccessResponse(
                new CategoryResponseDto
                {
                    Id = existing.Id,
                    Name = existing.Name
                },
                "Category updated successfully"
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, Guid adminId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Category not found");

            var deleted = await _repo.DeleteSoftAsync(id, adminId);

            return ApiResponse<bool>.SuccessResponse(deleted, "Category deleted successfully");
        }
    }
}
