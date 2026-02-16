using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.Todos.Queries.GetTodos;

public sealed class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, Result<PaginatedResult<TodoResponseDto>>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTodosQueryHandler(ITodoRepository todoRepository, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaginatedResult<TodoResponseDto>>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var result = await _todoRepository.GetPagedForUserAsync(_currentUserService.UserId, request.Request, cancellationToken);
        return Result<PaginatedResult<TodoResponseDto>>.Success(result);
    }
}
