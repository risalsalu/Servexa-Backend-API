using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Checkout;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : BaseController
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(InitiateCheckoutDto dto)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Success(await _checkoutService.PayAsync(dto, customerId));
        }
    }
}
