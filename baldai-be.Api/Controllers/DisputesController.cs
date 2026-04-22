using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace baldai_be.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class DisputesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DisputesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenDispute([FromBody] baldai_be.Application.DTOs.OpenDisputeDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var buyerId)) return Unauthorized();

        var disputeId = await _mediator.Send(new baldai_be.Application.Features.Disputes.OpenDisputeCommand(buyerId, request));
        return Created($"/api/v1/orders/{request.OrderId}", new { disputeId = disputeId, status = "Disputed" });
    }

    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> ResolveDispute(Guid id, [FromBody] baldai_be.Application.DTOs.ResolveDisputeDto request)
    {
        // Add moderator role check if necessary
        var newStatus = await _mediator.Send(new baldai_be.Application.Features.Disputes.ResolveDisputeCommand(id, request));
        return Ok(new { finalOrderState = newStatus });
    }
}
