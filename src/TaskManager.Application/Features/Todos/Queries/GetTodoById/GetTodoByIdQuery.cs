using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Features.Todos.Queries.GetTodoById;

public sealed record GetTodoByIdQuery(Guid TodoId) : IRequest<Result<TodoResponseDto>>;
