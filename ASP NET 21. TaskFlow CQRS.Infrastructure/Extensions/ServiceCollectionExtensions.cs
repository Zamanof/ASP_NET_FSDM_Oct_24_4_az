using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ASP_NET_21._TaskFlow_CQRS.Application.Config;
using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Auth;
using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Storage;
using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Persistence;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Services;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow_CQRS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        services.AddDbContext<TaskFlowDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddScoped<ITaskAttachmentRepository, TaskAttachmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthIdentityProvider, AuthIdentityProvider>();
        services.AddScoped<IFileStorage, LocalDiskStorage>();

        return services;
    }

    public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<TaskFlowDbContext>()
        .AddDefaultTokenProviders();

        var jwtConfig = new JwtConfig();
        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);
        services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer, ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey!)), ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
