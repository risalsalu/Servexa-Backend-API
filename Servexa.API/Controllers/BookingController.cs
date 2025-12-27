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
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("draft")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateDraft([FromBody] CreateBookingDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.CreateDraftAsync(userId, dto);
            return Success(result, "Booking draft created");
        }

        [HttpPut("{bookingId:guid}/address")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SelectAddress(Guid bookingId, [FromBody] SelectBookingAddressDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updated = await _bookingService.SelectAddressAsync(bookingId, dto.AddressId, userId);

            if (!updated)
                return BadRequestError("Address selection not allowed");

            return Success(true, "Address selected");
        }

        [HttpPut("{bookingId:guid}/slot")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SelectSlot(Guid bookingId, [FromBody] SelectBookingSlotDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updated = await _bookingService.SelectSlotAsync(bookingId, dto.SlotId, userId);

            if (!updated)
                return BadRequestError("Slot selection not allowed");

            return Success(true, "Slot selected");
        }

        [HttpGet("{bookingId:guid}/summary")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Summary(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetSummaryAsync(bookingId, userId);
            return Success(result, "Booking summary fetched");
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetByCustomerAsync(userId);
            return Success(result, "Bookings fetched");
        }

        [HttpDelete("{bookingId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cancelled = await _bookingService.CancelAsync(bookingId, userId);

            if (!cancelled)
                return BadRequestError("Booking cancellation not allowed");

            return Success(true, "Booking cancelled");
        }

        [HttpGet("shop")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> ShopBookings()
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.GetByShopAsync(ownerId);
            return Success(result, "Shop bookings fetched");
        }

        [HttpPut("status")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateBookingStatusDto dto)
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updated = await _bookingService.UpdateStatusAsync(dto.BookingId, dto.Status, ownerId);

            if (!updated)
                return BadRequestError("Booking status update not allowed");

            return Success(true, "Booking marked as completed");
        }
    }
}
