using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servexa.Infrastructure.Services
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ICategoryRepository _repo;

        public AdminCategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new CategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid adminId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");

            if (await _repo.ExistsByNameAsync(dto.Name))
                throw new Exception("Category name already exists");

            var entity = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                CreatedBy = adminId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repo.AddAsync(entity);

            return new CategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public async Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid adminId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Category not found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");

            if (await _repo.ExistsByNameExceptIdAsync(id, dto.Name))
                throw new Exception("Category name already exists");

            existing.Name = dto.Name;
            existing.ModifiedBy = adminId;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);

            return new CategoryResponseDto
            {
                Id = existing.Id,
                Name = existing.Name
            };
        }

        public async Task<bool> DeleteAsync(Guid id, Guid adminId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Category not found");

            return await _repo.DeleteSoftAsync(id, adminId);
        }
    }
}
