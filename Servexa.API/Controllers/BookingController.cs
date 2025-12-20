using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(Roles = "Customer")]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetByCustomerAsync(customerId);
            return Success(result, "Bookings fetched successfully");
        }

        [HttpGet("{bookingId:guid}")]
        public async Task<IActionResult> GetById(Guid bookingId)
        {
            var result = await _bookingService.GetByIdAsync(bookingId);
            return Success(result, "Booking fetched successfully");
        }

        [HttpDelete("{bookingId:guid}")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.CancelAsync(bookingId, customerId);
            return Success(result, "Booking cancelled successfully");
        }
    }
}
