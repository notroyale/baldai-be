using baldai_be.Application.DTOs;
using baldai_be.Application.Features.Chats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace baldai_be.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetInbox()
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetInboxQuery(userId));
        return Ok(result);
    }

    [HttpGet("{threadId}/messages")]
    public async Task<IActionResult> GetMessages(Guid threadId)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetThreadMessagesQuery(threadId, userId));
        return Ok(result);
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new SendMessageCommand(userId, request));
        return Ok(result);
    }

    [HttpPost("offers")]
    public async Task<IActionResult> MakeOffer([FromBody] CreateOfferDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var buyerId)) return Unauthorized();

        var result = await _mediator.Send(new CreateOfferCommand(buyerId, request));
        return Ok(result);
    }

    [HttpPost("offers/{offerId}/accept")]
    public async Task<IActionResult> AcceptOffer(Guid offerId)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        await _mediator.Send(new AcceptOfferCommand(offerId, sellerId));
        return Ok(new { success = true, status = "Accepted" });
    }

    [HttpPost("offers/{offerId}/decline")]
    public async Task<IActionResult> DeclineOffer(Guid offerId)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        await _mediator.Send(new DeclineOfferCommand(offerId, sellerId));
        return Ok(new { success = true, status = "Rejected" });
    }
}
