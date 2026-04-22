using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace baldai_be.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModerationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _mediator.Send(new baldai_be.Application.Features.Moderation.GetPendingProductsQuery());
        return Ok(result);
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _mediator.Send(new baldai_be.Application.Features.Moderation.ApproveProductCommand(id));
        return Ok(new { success = true, newStatus = "Active" });
    }

    [HttpPost("reject/{id}")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] baldai_be.Application.DTOs.RejectDto request)
    {
        await _mediator.Send(new baldai_be.Application.Features.Moderation.RejectProductCommand(id, request));
        return Ok(new { success = true, newStatus = "Rejected" });
    }
}
