using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;
