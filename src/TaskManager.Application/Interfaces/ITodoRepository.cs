using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdForUserAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default);

    Task<PaginatedResult<TodoResponseDto>> GetPagedForUserAsync(
        Guid userId,
        GetTodosRequest request,
        CancellationToken cancellationToken = default);

    Task AddAsync(TodoItem item, CancellationToken cancellationToken = default);

    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);

    Task DeleteAsync(TodoItem item, CancellationToken cancellationToken = default);
}
