using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace baldai_be.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class LogisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{orderId}/pickup-code")]
    public async Task<IActionResult> GeneratePickupCode(Guid orderId)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var buyerId)) return Unauthorized();

        var code = await _mediator.Send(new baldai_be.Application.Features.Logistics.GeneratePickupCodeCommand(orderId, buyerId));
        return Ok(new { PickupCode = code });
    }

    [HttpPost("{orderId}/verify-pickup")]
    public async Task<IActionResult> VerifyPickupCode(Guid orderId, [FromBody] baldai_be.Application.DTOs.VerifyPickupDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        await _mediator.Send(new baldai_be.Application.Features.Logistics.VerifyPickupCommand(orderId, sellerId, request));
        return Ok(new { success = true, status = "Delivered" });
    }
}
