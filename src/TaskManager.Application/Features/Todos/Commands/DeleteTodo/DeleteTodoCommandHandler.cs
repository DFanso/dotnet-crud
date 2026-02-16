using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.Todos.Commands.DeleteTodo;

public sealed class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Result>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdForUserAsync(request.TodoId, _currentUserService.UserId, cancellationToken);
        if (todo is null)
        {
            return Result.Failure("Todo not found.");
        }

        await _todoRepository.DeleteAsync(todo, cancellationToken);
        return Result.Success();
    }
}
