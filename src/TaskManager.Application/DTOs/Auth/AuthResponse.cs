namespace TaskManager.Application.DTOs.Auth;

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, string FullName, string Email);
