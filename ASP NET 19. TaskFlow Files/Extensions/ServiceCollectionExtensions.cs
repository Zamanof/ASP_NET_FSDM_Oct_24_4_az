using ASP_NET_19._TaskFlow_Files.Authorization;
using ASP_NET_19._TaskFlow_Files.Config;
using ASP_NET_19._TaskFlow_Files.Data;
using ASP_NET_19._TaskFlow_Files.Mappings;
using ASP_NET_19._TaskFlow_Files.Models;
using ASP_NET_19._TaskFlow_Files.Services;
using ASP_NET_19._TaskFlow_Files.Storage;
using ASP_NET_19._TaskFlow_Files.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ASP_NET_19._TaskFlow_Files.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSwagger(
        this IServiceCollection services
        )
    {
        services.AddControllers();

        services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "TaskFlow API",
            Description = "API for managing projects and tasks. This API includes all CRUD operations for managing projects and tasks.",
            Contact = new OpenApiContact
            {
                Name = "TaskFlow Team",
                Email = "support@taslflow.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT Licence",
                Url = new Uri("https://opensource.org/license/mit")
            }

        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);


        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
                JWT Suthorization header using the Bearer scheme. 
                Example: Authorization: Bearer {token}
                """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
            });
    }

    );
        return services;
    }


    public static IServiceCollection AddTaskFlowDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
                        .GetConnectionString("DefaultConnectionString");

        services.AddDbContext<TaskFlowDbContext>(
            options => options.UseSqlServer(connectionString)
            );
        return services;
    }

    public static IServiceCollection AddIdentityAndDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    }

)
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddJwtAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();

        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);


        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        // Authorization policies
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy
                        => policy.RequireRole("Admin"));

                options.AddPolicy(
                    "AdminOrManager",
                    policy
                        => policy.RequireRole("Admin", "Manager"));

                options.AddPolicy(
                    "UserOrAbove",
                    policy
                        => policy.RequireRole("Admin", "Manager", "User"));


                options.AddPolicy(
                    "ProjectOwnerOrAdmin",
                        policy =>
                            policy.Requirements.Add(new ProjectOwnerOrAdminRequirment()));


                options.AddPolicy(
                    "ProjectMemberOrHigher",
                        policy =>
                        policy.Requirements.Add(new ProjectMemberOrHigherRequirment()));


                options.AddPolicy(
                    "TaskStatusChange",
                    policy =>
                        policy.Requirements.Add(new TaskStatusChangeRequirment()));
            });
        services.AddScoped<IAuthorizationHandler, ProjectOwnerOrAdminHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberOrHigherHandler>();
        services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();

        return services;

    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services)
    {
        services.AddCors(
    optons =>
    {
        optons.AddDefaultPolicy(
            policy => policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            );
    }
    );
        return services;
    }

    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }

    public static IServiceCollection AddAutoMapperAndOtherServices(
        this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileStorage, LocalDiskStorage>();
        services.AddScoped<IAttachmentService, AttachmentService>();

        return services;

    }

}
