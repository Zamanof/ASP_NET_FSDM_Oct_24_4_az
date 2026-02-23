using TaskFlow.Api.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwagger()
    .AddFluentValidation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddIdentityAndJwt(builder.Configuration)
    .AddAuthorizationPolicies()
    .AddCorsPolicy();

var app = builder.Build();

app.UseTaskFlowPipeline();
await app.EnsureRolesSeededAsync();

app.Run();
