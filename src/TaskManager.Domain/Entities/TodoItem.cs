using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public sealed class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    public TodoStatus Status { get; set; } = TodoStatus.Pending;

    public DateTimeOffset? DueDate { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }

    public bool IsDeleted { get; set; }
}
