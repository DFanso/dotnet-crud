using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Authentication;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableDetailedErrors();
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
