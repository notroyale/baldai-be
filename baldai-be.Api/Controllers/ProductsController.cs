using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using baldai_be.Application.DTOs;

namespace baldai_be.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] decimal? maxPrice = null)
    {
        var result = await _mediator.Send(new baldai_be.Application.Features.Products.GetProductsQuery(page, pageSize, search, maxPrice));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        var productId = await _mediator.Send(new baldai_be.Application.Features.Products.CreateProductCommand(sellerId, request));
        return Created($"/api/v1/products/{productId}", new { productId = productId, status = "PendingModeration" });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new baldai_be.Application.Features.Products.GetProductByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductCreateDto request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var sellerId)) return Unauthorized();

        await _mediator.Send(new baldai_be.Application.Features.Products.UpdateProductCommand(id, sellerId, request));
        return Ok(new { productId = id, status = "PendingModeration" });
    }
}
