using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.DTOs.Shop;
using Servexa.Application.Interfaces;
using Servexa.Shared.Responses;

namespace Servexa.API.Controllers;

[ApiController]
[Route("api/shop")]
[Authorize(Roles = "ShopOwner")]
public class ShopController : ControllerBase
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<Guid>>> Register([FromBody] AddShopDto dto)
    {
        var ownerId = GetUserId();
        var response = await _shopService.RegisterShopAsync(ownerId, dto);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<ShopResponseDto>>> Get()
    {
        var ownerId = GetUserId();
        var response = await _shopService.GetShopAsync(ownerId);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<bool>>> Update([FromBody] UpdateShopDto dto)
    {
        var ownerId = GetUserId();
        var response = await _shopService.UpdateShopAsync(ownerId, dto);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
    }

    [HttpPatch("activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate([FromBody] ActivateShopDto dto)
    {
        var ownerId = GetUserId();
        var response = await _shopService.SetActiveStatusAsync(ownerId, dto.IsActive);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
    }

    [HttpPost("images")]
    public async Task<ActionResult<ApiResponse<AddShopImageDto>>> AddImage(IFormFile file)
    {
        var ownerId = GetUserId();
        var response = await _shopService.AddShopImageAsync(ownerId, file);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
    }

    [HttpDelete("images/{imageId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImage(Guid imageId)
    {
        var ownerId = GetUserId();
        var response = await _shopService.DeleteShopImageAsync(ownerId, imageId);
        return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }
}
