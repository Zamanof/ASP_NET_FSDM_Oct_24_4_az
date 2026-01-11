using ASP_NET_02._Mini_ASP.Interfaces;
using ASP_NET_02._Mini_ASP.Middlewares;

namespace ASP_NET_02._Mini_ASP;

class Startup : IStartup
{
    public void Configure(MiddlewareBuilder builder)
    {
        builder.Use<LoggerMiddleware>();
        builder.Use<StaticFileMiddlware>();
    }
}
