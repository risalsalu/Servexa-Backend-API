using Servexa.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servexa.Application.Interfaces
{
    public interface IAdminCategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid adminId);
        Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid adminId);
        Task<bool> DeleteAsync(Guid id, Guid adminId);
    }
}
