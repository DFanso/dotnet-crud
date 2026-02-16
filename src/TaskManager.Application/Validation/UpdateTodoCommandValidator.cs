using FluentValidation;
using TaskManager.Application.Features.Todos.Commands.UpdateTodo;

namespace TaskManager.Application.Validation;

public sealed class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.TodoId).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new UpdateTodoDtoValidator());
    }
}
