using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.Todos.Commands.UpdateTodo;

public sealed class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Result<TodoResponseDto>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTodoCommandHandler(ITodoRepository todoRepository, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TodoResponseDto>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdForUserAsync(request.TodoId, _currentUserService.UserId, cancellationToken);
        if (todo is null)
        {
            return Result<TodoResponseDto>.Failure("Todo not found.");
        }

        todo.Title = request.Request.Title.Trim();
        todo.Description = request.Request.Description?.Trim();
        todo.Priority = request.Request.Priority;
        todo.Status = request.Request.Status;
        todo.DueDate = request.Request.DueDate;
        todo.UpdatedAt = DateTimeOffset.UtcNow;

        await _todoRepository.UpdateAsync(todo, cancellationToken);
        return Result<TodoResponseDto>.Success(todo.ToResponse());
    }
}
