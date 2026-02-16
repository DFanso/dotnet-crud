using FluentValidation;
using TaskManager.Application.Features.Todos.Commands.CreateTodo;

namespace TaskManager.Application.Validation;

public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreateTodoDtoValidator());
    }
}
