using Microsoft.AspNetCore.Mvc;
using MediatR;
using baldai_be.Application.DTOs;

namespace baldai_be.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await _mediator.Send(new baldai_be.Application.Features.Auth.RegisterCommand(request));
        return Created("", result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await _mediator.Send(new baldai_be.Application.Features.Auth.LoginCommand(request));
        return Ok(result);
    }
}
