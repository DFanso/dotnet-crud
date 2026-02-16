using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (existing is not null)
        {
            return Result<AuthResponse>.Failure("Email is already registered.");
        }

        var user = new User
        {
            FullName = request.Request.FullName.Trim(),
            Email = request.Request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(request.Request.Password)
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var tokenResult = _jwtService.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(tokenResult.AccessToken, tokenResult.ExpiresAt, user.FullName, user.Email));
    }
}
