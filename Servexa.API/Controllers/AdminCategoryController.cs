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
    public class AdminCategoryController : ControllerBase
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
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            return Ok(await _service.CreateAsync(dto, AdminId()));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryDto dto)
        {
            return Ok(await _service.UpdateAsync(id, dto, AdminId()));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _service.DeleteAsync(id, AdminId()));
        }
    }
}
