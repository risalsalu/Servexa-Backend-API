using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.API.Controllers;
using Servexa.Application.DTOs.Address;
using Servexa.Application.Interfaces;
using System.Security.Claims;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/customers/addresses")]
    [Authorize(Roles = "Customer")]
    public class CustomerAddressController : BaseController
    {
        private readonly ICustomerAddressService _service;

        public CustomerAddressController(ICustomerAddressService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(ClaimTypes.Name)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.Parse(id!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var result = await _service.GetAddressesAsync(userId);
            return Success(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddAddressDto dto)
        {
            var userId = GetUserId();
            var id = await _service.AddAddressAsync(userId, dto);
            return Success(new { addressId = id }, "Address created");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressDto dto)
        {
            var userId = GetUserId();
            var ok = await _service.UpdateAddressAsync(userId, id, dto);
            if (!ok) return Error("Address not found or not owned by user");
            return SuccessMessage("Address updated");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var ok = await _service.DeleteAddressAsync(userId, id);
            if (!ok) return Error("Address not found or not owned by user");
            return SuccessMessage("Address deleted");
        }
    }
}
