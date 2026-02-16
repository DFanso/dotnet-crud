using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Todos;

public sealed record TodoResponseDto(
    Guid Id,
    string Title,
    string? Description,
    TodoPriority Priority,
    TodoStatus Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
