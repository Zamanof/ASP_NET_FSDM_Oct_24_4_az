using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Storage;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ASP_NET_21._TaskFlow_CQRS_Integration_tests;

public class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDB"] = "True"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IFileStorage>();
            services.TryAddSingleton<IFileStorage, TestFileStorage>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        context.Database.EnsureCreated();
        RoleSeeder.SeedRolesAsync(scope.ServiceProvider).GetAwaiter().GetResult();
        return host;
    }
}
