using FluentValidation;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Validation;

public sealed class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
{
    public UpdateTodoDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
