using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAddressesAsync(GetUserId());
            return Success(result, "Addresses fetched");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddAddressDto dto)
        {
            var id = await _service.AddAddressAsync(GetUserId(), dto);
            return Created(new { addressId = id }, "Address created");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressDto dto)
        {
            var ok = await _service.UpdateAddressAsync(GetUserId(), id, dto);
            if (!ok)
                return NotFoundError("Address not found");

            return SuccessMessage("Address updated");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAddressAsync(GetUserId(), id);
            if (!ok)
                return NotFoundError("Address not found");

            return SuccessMessage("Address deleted");
        }
    }
}
