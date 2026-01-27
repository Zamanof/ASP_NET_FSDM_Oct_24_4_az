using ASP_NET_09._TaskFlow_Swagger_Documentation.Data;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Mappings;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Services;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
    }
    );

var connectionString = builder
                        .Configuration
                        .GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<TaskFlowDbContext>(
    options => options.UseSqlServer(connectionString)
    );

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();

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
        }
        );
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
