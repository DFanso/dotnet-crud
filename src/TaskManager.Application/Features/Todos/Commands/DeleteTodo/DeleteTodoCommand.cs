using MediatR;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Todos.Commands.DeleteTodo;

public sealed record DeleteTodoCommand(Guid TodoId) : IRequest<Result>;
