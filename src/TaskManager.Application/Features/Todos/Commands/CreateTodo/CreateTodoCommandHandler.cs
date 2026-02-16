using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Todos.Commands.CreateTodo;

public sealed class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Result<TodoResponseDto>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTodoCommandHandler(ITodoRepository todoRepository, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TodoResponseDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Title = request.Request.Title.Trim(),
            Description = request.Request.Description?.Trim(),
            Priority = request.Request.Priority,
            DueDate = request.Request.DueDate,
            UserId = _currentUserService.UserId
        };

        await _todoRepository.AddAsync(todo, cancellationToken);
        return Result<TodoResponseDto>.Success(todo.ToResponse());
    }
}
