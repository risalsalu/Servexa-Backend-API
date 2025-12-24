using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/shop/bookings")]
    [Authorize(Roles = "ShopOwner")]
    public class ShopBookingsController : BaseController
    {
        private readonly IShopBookingReadService _service;

        public ShopBookingsController(IShopBookingReadService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _service.GetShopBookingsAsync(ownerId);
            return Success(result);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateBookingStatusDto dto)
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _service.UpdateBookingStatusAsync(ownerId, dto);
            return Success(true, "Booking status updated successfully");
        }
    }
}
