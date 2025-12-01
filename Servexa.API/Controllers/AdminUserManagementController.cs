using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;
using System.Security.Claims;


namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/user-management")]
    public class AdminUserManagementController : ControllerBase
    {
        private readonly IAdminUserManagementService _service;

        public AdminUserManagementController(IAdminUserManagementService service)
        {
            _service = service;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _service.GetAllUsersAsync());
        }

        [HttpGet("shopowners")]
        public async Task<IActionResult> GetShopOwners()
        {
            return Ok(await _service.GetAllShopOwnersAsync());
        }

        [HttpPut("customers/{id:guid}/status")]
        public async Task<IActionResult> SetCustomerStatus(Guid id, [FromQuery] bool isActive)
        {
            return Ok(await _service.SetUserActiveStatusAsync(id, isActive));
        }

        [HttpPut("shopowners/{id:guid}/status")]
        public async Task<IActionResult> SetShopOwnerStatus(Guid id, [FromQuery] bool isActive)
        {
            return Ok(await _service.SetShopOwnerActiveStatusAsync(id, isActive));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null)
                return Unauthorized();

            var adminId = Guid.Parse(adminIdClaim.Value);
            return Ok(await _service.DeleteUserAsync(id, adminId));
        }

    }
}
