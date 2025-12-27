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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateSlotDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var slotId = await _slotService.CreateSlotAsync(dto, customerId);
            return Success(slotId);
        }

        [HttpGet("shop/{shopId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Get(Guid shopId, [FromQuery] DateTime date)
        {
            var result = await _slotService.GetAvailableSlotsAsync(shopId, date);
            return Success(result);
        }
    }
}
