using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingFromCartDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Success(await _bookingService.CreateAsync(customerId, dto));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Summary(Guid id)
        {
            return Success(await _bookingService.GetSummaryAsync(id));
        }
    }
}
