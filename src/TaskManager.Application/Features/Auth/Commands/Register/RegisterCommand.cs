using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(RegisterRequest Request) : IRequest<Result<AuthResponse>>;
