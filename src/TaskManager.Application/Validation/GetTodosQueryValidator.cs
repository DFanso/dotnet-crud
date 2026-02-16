using FluentValidation;
using TaskManager.Application.Features.Todos.Queries.GetTodos;

namespace TaskManager.Application.Validation;

public sealed class GetTodosQueryValidator : AbstractValidator<GetTodosQuery>
{
    public GetTodosQueryValidator()
    {
        RuleFor(x => x.Request.PageNumber).GreaterThan(0);
        RuleFor(x => x.Request.PageSize).InclusiveBetween(1, 100);
    }
}
