namespace TaskManager.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
