using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASP_NET_20._TaskFlow.Application;

public static class  DI
{

    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services;
    }
}

