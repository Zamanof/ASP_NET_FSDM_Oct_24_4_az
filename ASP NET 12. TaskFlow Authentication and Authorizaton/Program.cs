using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Data;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Mappings;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Middleware;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Models;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

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
            },
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
    }
    );

var connectionString = builder
                        .Configuration
                        .GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<TaskFlowDbContext>(
    options => options.UseSqlServer(connectionString)
    );

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskFlowDbContext>();

// Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#region FluentValidation DI
//builder.Services.AddScoped<IValidator<CreateProjectRequest>, CreateProjectValidator>();
//builder.Services.AddScoped<IValidator<UpdateProjectRequest>, UpdateProjectValidator>();
//builder.Services.AddScoped<IValidator<CreateTaskItemRequest>, CreateTaskItemValidator>();
//builder.Services.AddScoped<IValidator<UpdateTaskItemRequest>, UpdateTaskItemValidator>();
#endregion

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options=>
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
app.UseAuthorization();

app.MapControllers();

app.Run();
