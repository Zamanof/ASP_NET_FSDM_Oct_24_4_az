using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using ASP_NET_21._TaskFlow_CQRS.Api.Authorization;
using ASP_NET_21._TaskFlow_CQRS.Application.Extensions;
using ASP_NET_21._TaskFlow_CQRS.Application.Validators;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Extensions;

namespace ASP_NET_21._TaskFlow_CQRS.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "TaskFlow API (Onion)", Description = "API for managing projects and tasks." });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Description = "JWT Bearer. Example: Authorization: Bearer {token}", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
        });
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            options.AddPolicy("AdminOrManager", p => p.RequireRole("Admin", "Manager"));
            options.AddPolicy("UserOrAbove", p => p.RequireRole("Admin", "Manager", "User"));
            options.AddPolicy("ProjectOwnerOrAdmin", p => p.Requirements.Add(new ProjectOwnerOrAdminRequirement()));
            options.AddPolicy("ProjectMemberOrHigher", p => p.Requirements.Add(new ProjectMemberOrHigherRequirement()));
            options.AddPolicy("TaskStatusChange", p => p.Requirements.Add(new TaskStatusChangeRequirement()));
        });
        services.AddScoped<IAuthorizationHandler, ProjectOwnerOrAdminHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberOrHigherHandler>();
        services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();
        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
        return services;
    }
}
