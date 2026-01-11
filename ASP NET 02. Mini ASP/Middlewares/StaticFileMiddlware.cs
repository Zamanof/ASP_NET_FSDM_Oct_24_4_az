using ASP_NET_02._Mini_ASP.Interfaces;
using System.IO;
using System.Net;

namespace ASP_NET_02._Mini_ASP.Middlewares;

class StaticFileMiddlware : IMiddleware
{
    public HttpHandler Next { get; set; }

    public void Handle(HttpListenerContext context)
    {
        var basePath = @"..\..\..\wwwroot\";
        if (Path.HasExtension(context.Request.RawUrl))
        {
            try
            {
                // /home.html
                // "" "home.html"
                // 0   1
                var fileName = context.Request.RawUrl.Substring(1);
                var path = $"{basePath}{fileName}";
                var bytes = File.ReadAllBytes(path);
                if (Path.GetExtension(path) == ".html")
                {
                    context.Response.AddHeader("Content-Type", "text/html");
                } else if (Path.GetExtension(path) == ".png")
                {
                    context.Response.AddHeader("Content-Type", "image/png");
                }
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "File not found";
                var bytes = File.ReadAllBytes($"{basePath}404.html");
                context.Response.AddHeader("Content-Type", "text/html");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }
        else
        {
            Next.Invoke(context);
        }
        context.Response.Close();
    }
}
