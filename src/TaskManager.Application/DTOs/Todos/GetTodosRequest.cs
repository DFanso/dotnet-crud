using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Todos;

public sealed record GetTodosRequest(
    int PageNumber = 1,
    int PageSize = 10,
    TodoStatus? Status = null,
    TodoPriority? Priority = null,
    DateTimeOffset? DueDateFrom = null,
    DateTimeOffset? DueDateTo = null,
    string? Search = null,
    string SortBy = "createdAt",
    bool Descending = true);
