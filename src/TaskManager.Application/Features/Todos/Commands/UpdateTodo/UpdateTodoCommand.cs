using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Features.Todos.Commands.UpdateTodo;

public sealed record UpdateTodoCommand(Guid TodoId, UpdateTodoDto Request) : IRequest<Result<TodoResponseDto>>;
