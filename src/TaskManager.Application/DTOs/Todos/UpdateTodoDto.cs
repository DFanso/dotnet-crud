using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Todos;

public sealed record UpdateTodoDto(
    string Title,
    string? Description,
    TodoPriority Priority,
    TodoStatus Status,
    DateTimeOffset? DueDate);
