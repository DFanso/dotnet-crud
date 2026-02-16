using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.Todos.Queries.GetTodoById;

public sealed class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, Result<TodoResponseDto>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TodoResponseDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdForUserAsync(request.TodoId, _currentUserService.UserId, cancellationToken);
        return todo is null
            ? Result<TodoResponseDto>.Failure("Todo not found.")
            : Result<TodoResponseDto>.Success(todo.ToResponse());
    }
}
