using FluentValidation;
using TaskManager.Application.Features.Auth.Commands.Register;

namespace TaskManager.Application.Validation;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new RegisterRequestValidator());
    }
}
