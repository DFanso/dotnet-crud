using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Todos;

public sealed record CreateTodoDto(
    string Title,
    string? Description,
    TodoPriority Priority,
    DateTimeOffset? DueDate);
