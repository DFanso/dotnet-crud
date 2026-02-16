using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public sealed class TodoRepository : ITodoRepository
{
    private static readonly Func<AppDbContext, Guid, Guid, IAsyncEnumerable<TodoItem?>> GetByIdCompiledQuery =
        EF.CompileAsyncQuery((AppDbContext db, Guid todoId, Guid userId) =>
            db.TodoItems
                .AsTracking()
                .Where(x => x.Id == todoId && x.UserId == userId)
                .Select(x => (TodoItem?)x)
                .Take(1));

    private readonly AppDbContext _dbContext;

    public TodoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TodoItem?> GetByIdForUserAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default)
    {
        await foreach (var item in GetByIdCompiledQuery(_dbContext, todoId, userId).WithCancellation(cancellationToken))
        {
            return item;
        }

        return null;
    }

    public async Task<PaginatedResult<TodoResponseDto>> GetPagedForUserAsync(
        Guid userId,
        GetTodosRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TodoItems
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == request.Priority.Value);
        }

        if (request.DueDateFrom.HasValue)
        {
            query = query.Where(x => x.DueDate >= request.DueDateFrom.Value);
        }

        if (request.DueDateTo.HasValue)
        {
            query = query.Where(x => x.DueDate <= request.DueDateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Title.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        }

        query = (request.SortBy.ToLowerInvariant(), request.Descending) switch
        {
            ("duedate", true) => query.OrderByDescending(x => x.DueDate),
            ("duedate", false) => query.OrderBy(x => x.DueDate),
            ("priority", true) => query.OrderByDescending(x => x.Priority),
            ("priority", false) => query.OrderBy(x => x.Priority),
            ("status", true) => query.OrderByDescending(x => x.Status),
            ("status", false) => query.OrderBy(x => x.Status),
            ("updatedat", true) => query.OrderByDescending(x => x.UpdatedAt),
            ("updatedat", false) => query.OrderBy(x => x.UpdatedAt),
            (_, true) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var pageSize = request.PageSize;
        var pageNumber = request.PageNumber;
        var skip = (pageNumber - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new TodoResponseDto(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.Status,
                x.DueDate,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TodoResponseDto>(items, pageNumber, pageSize, totalCount);
    }

    public async Task AddAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        await _dbContext.TodoItems.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.TodoItems.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        item.IsDeleted = true;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        _dbContext.TodoItems.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
