using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Admin;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/category")]
    public class AdminCategoryController : BaseController
    {
        private readonly IAdminCategoryService _service;

        public AdminCategoryController(IAdminCategoryService service)
        {
            _service = service;
        }

        private Guid AdminId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Success(result, "Categories fetched successfully");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var result = await _service.CreateAsync(dto, AdminId());
            return Success(result, "Category created successfully");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryDto dto)
        {
            var result = await _service.UpdateAsync(id, dto, AdminId());
            return Success(result, "Category updated successfully");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id, AdminId());
            return Success(result, "Category deleted successfully");
        }
    }
}
