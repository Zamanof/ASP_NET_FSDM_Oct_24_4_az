using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Authorization;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Data;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Mappings;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Middleware;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(
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

var connectionString = builder
                        .Configuration
                        .GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<TaskFlowDbContext>(
    options => options.UseSqlServer(connectionString)
    );

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    }

)
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(
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
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                ClockSkew = TimeSpan.Zero
            };
        }
    );

// Authorization policies
builder.Services.AddAuthorization(
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
    }
    );


// Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectOwnerOrAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectMemberOrHigherHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();

#region FluentValidation DI
//builder.Services.AddScoped<IValidator<CreateProjectRequest>, CreateProjectValidator>();
//builder.Services.AddScoped<IValidator<UpdateProjectRequest>, UpdateProjectValidator>();
//builder.Services.AddScoped<IValidator<CreateTaskItemRequest>, CreateTaskItemValidator>();
//builder.Services.AddScoped<IValidator<UpdateTaskItemRequest>, UpdateTaskItemValidator>();
#endregion

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddCors(
    optons =>
    {
        optons.AddDefaultPolicy(
            policy=> policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000" )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            );
    }
    );

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskFlow API v1");
            options.RoutePrefix = string.Empty;
            options.DisplayRequestDuration();
            options.EnableFilter();
            options.EnableDeepLinking();
            options.EnableTryItOutByDefault();
        }
        );
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await RoleSeeder.SeedRolesAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured while seeding roles");
    }
}

app.Run();
