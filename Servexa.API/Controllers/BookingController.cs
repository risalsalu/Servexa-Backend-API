using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Booking;
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

        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft([FromBody] CreateBookingDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.CreateDraftAsync(customerId, dto);
            return Success(result, "Booking draft created");
        }

        [HttpPut("{bookingId:guid}/address")]
        public async Task<IActionResult> SelectAddress(Guid bookingId, [FromBody] SelectBookingAddressDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.SelectAddressAsync(bookingId, dto.AddressId, customerId);
            return Success(result, "Address selected");
        }

        [HttpPut("{bookingId:guid}/slot")]
        public async Task<IActionResult> SelectSlot(Guid bookingId, [FromBody] SelectBookingSlotDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.SelectSlotAsync(bookingId, dto.SlotId, customerId);
            return Success(result, "Slot selected");
        }

        [HttpGet("{bookingId:guid}/summary")]
        public async Task<IActionResult> GetSummary(Guid bookingId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetSummaryAsync(bookingId, customerId);
            return Success(result, "Booking summary fetched");
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetByCustomerAsync(customerId);
            return Success(result, "Bookings fetched successfully");
        }

        [HttpDelete("{bookingId:guid}")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.CancelAsync(bookingId, customerId);
            return Success(result, "Booking cancelled");
        }
    }
}

