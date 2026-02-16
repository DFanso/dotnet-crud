using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Features.Todos.Commands.CreateTodo;

public sealed record CreateTodoCommand(CreateTodoDto Request) : IRequest<Result<TodoResponseDto>>;
