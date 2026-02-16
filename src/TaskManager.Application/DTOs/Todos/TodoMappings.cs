using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs.Todos;

public static class TodoMappings
{
    public static TodoResponseDto ToResponse(this TodoItem item)
    {
        return new TodoResponseDto(
            item.Id,
            item.Title,
            item.Description,
            item.Priority,
            item.Status,
            item.DueDate,
            item.CreatedAt,
            item.UpdatedAt);
    }
}
