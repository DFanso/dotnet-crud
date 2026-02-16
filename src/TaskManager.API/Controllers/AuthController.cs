using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Features.Auth.Commands.Login;
using TaskManager.Application.Features.Auth.Commands.Register;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterCommand(request), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LoginCommand(request), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }
}
