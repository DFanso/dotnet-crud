using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        var tokenResult = _jwtService.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(tokenResult.AccessToken, tokenResult.ExpiresAt, user.FullName, user.Email));
    }
}
