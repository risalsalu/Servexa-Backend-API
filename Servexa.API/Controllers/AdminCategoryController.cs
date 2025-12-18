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
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.Success)
                return BadRequestError(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _service.CreateAsync(dto, AdminId());
            if (!result.Success)
                return BadRequestError(result.Message);

            return Created(result.Data, result.Message);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _service.UpdateAsync(id, dto, AdminId());
            if (!result.Success)
                return BadRequestError(result.Message);

            return Success(result.Data, result.Message);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id, AdminId());
            if (!result.Success)
                return BadRequestError(result.Message);

            return Success(result.Data, result.Message);
        }
    }
}
