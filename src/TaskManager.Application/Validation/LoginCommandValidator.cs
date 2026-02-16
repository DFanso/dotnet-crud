using FluentValidation;
using TaskManager.Application.Features.Auth.Commands.Login;

namespace TaskManager.Application.Validation;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new LoginRequestValidator());
    }
}
