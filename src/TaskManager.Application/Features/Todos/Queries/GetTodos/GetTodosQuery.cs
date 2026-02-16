using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;

namespace TaskManager.Application.Features.Todos.Queries.GetTodos;

public sealed record GetTodosQuery(GetTodosRequest Request) : IRequest<Result<PaginatedResult<TodoResponseDto>>>;
