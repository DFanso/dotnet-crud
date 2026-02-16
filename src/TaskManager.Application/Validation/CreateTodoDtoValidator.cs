using FluentValidation;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Validation;

public sealed class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
