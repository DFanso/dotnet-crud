using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface IJwtService
{
    (string AccessToken, DateTimeOffset ExpiresAt) GenerateToken(User user);
}
