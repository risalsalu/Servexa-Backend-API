using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Slot;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/slots")]
    public class SlotController : BaseController
    {
        private readonly ISlotService _slotService;

        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpPost]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> Create([FromBody] CreateSlotDto dto)
        {
            if (dto == null)
                return BadRequestError("Invalid request body");

            if (dto.Date == default)
                return BadRequestError("Date is required");

            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await _slotService.CreateSlotsAsync(dto, ownerId);
            return Success(count, "Slots created successfully");
        }

        [HttpGet("shop/{shopId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Get(Guid shopId, [FromQuery] DateTime date)
        {
            if (date == default)
                return BadRequestError("Date is required");

            var result = await _slotService.GetAvailableSlotsAsync(shopId, date);
            return Success(result, "Slots fetched successfully");
        }

        [HttpDelete("{slotId:guid}")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> Delete(Guid slotId)
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _slotService.DeleteSlotAsync(slotId, ownerId);
            return Success(result, "Slot deleted successfully");
        }
    }
}
