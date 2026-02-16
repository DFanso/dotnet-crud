using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskManager.Infrastructure.Authentication;

namespace TaskManager.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager API", Version = "v1" });
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            });
        });

        services.AddHealthChecks();
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", policy =>
            {
                policy.PermitLimit = 100;
                policy.Window = TimeSpan.FromMinutes(1);
                policy.QueueLimit = 0;
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        return services;
    }
}
