using Servexa.Application.DTOs.Admin;
using Servexa.Shared.Responses;

namespace Servexa.Application.Interfaces
{
    public interface IAdminCategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllAsync();
        Task<ApiResponse<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto, Guid adminId);
        Task<ApiResponse<CategoryResponseDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid adminId);
        Task<ApiResponse<bool>> DeleteAsync(Guid id, Guid adminId);
    }
}
