using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using baldai_be.Application.DTOs;

namespace baldai_be.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var buyerId)) return Unauthorized();

        var result = await _mediator.Send(new baldai_be.Application.Features.Orders.CheckoutCommand(buyerId, request));
        return Created($"/api/v1/orders/{result.TransactionId}", result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new baldai_be.Application.Features.Orders.GetOrderStateQuery(id, userId));
        return Ok(result);
    }

    [HttpPost("{id}/shipping")]
    public async Task<IActionResult> MarkShipped(Guid id, [FromBody] ShippingUpdateDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        await _mediator.Send(new baldai_be.Application.Features.Orders.MarkShippedCommand(id, sellerId, request));
        return Ok(new { status = "Shipped" });
    }

    [HttpPatch("{id}/delivered")]
    [AllowAnonymous] // Webhooks are often anonymous or verified via header signature
    public async Task<IActionResult> WebhookDelivered(Guid id)
    {
        var escrowRelease = await _mediator.Send(new baldai_be.Application.Features.Orders.MarkDeliveredCommand(id));
        return Ok(new { status = "Delivered", escrowReleaseDate = escrowRelease.ToString("O") });
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var buyerId)) return Unauthorized();

        await _mediator.Send(new baldai_be.Application.Features.Orders.AcceptOrderCommand(id, buyerId));
        return Ok(new { status = "Completed", payoutInitiated = true });
    }
}
