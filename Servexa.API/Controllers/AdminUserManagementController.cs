using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/user-management")]
    public class AdminUserManagementController : BaseController
    {
        private readonly IAdminUserManagementService _service;

        public AdminUserManagementController(IAdminUserManagementService service)
        {
            _service = service;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _service.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("shopowners")]
        public async Task<IActionResult> GetShopOwners()
        {
            var result = await _service.GetAllShopOwnersAsync();
            return Ok(result);
        }

        [HttpPut("customers/{id:guid}/status")]
        public async Task<IActionResult> SetCustomerStatus(Guid id, [FromQuery] bool isActive)
        {
            var result = await _service.SetUserActiveStatusAsync(id, isActive);
            return Ok(result);
        }

        [HttpPut("shopowners/{id:guid}/status")]
        public async Task<IActionResult> SetShopOwnerStatus(Guid id, [FromQuery] bool isActive)
        {
            var result = await _service.SetShopOwnerActiveStatusAsync(id, isActive);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.DeleteUserAsync(id, adminId);
            return Ok(result);
        }
    }
}
