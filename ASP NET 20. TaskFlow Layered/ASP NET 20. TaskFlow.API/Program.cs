using ASP_NET_20._TaskFlow.API.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwagger()
    .AddFluentValidation()
    .AddTaskFlowDbContext(builder.Configuration)
    .AddIdentityAndDb(builder.Configuration)
    .AddJwtAuthenticationAndAuthorization(builder.Configuration)
    .AddCorsPolicy()
    .AddAutoMapperAndOtherDI();

var app = builder.Build();

app.UseTaskFlowPipeline();
await app.EnsureRolesSeededAsync();

app.Run();
