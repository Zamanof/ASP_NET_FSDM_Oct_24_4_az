using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASP_NET_20._TaskFlow.Infrastructure;

public static class  DI
{

    public static IServiceCollection AddInfrastructe(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services;
    }
}

