using System.Net;

namespace ASP_NET_02._Mini_HTTP_Server;

class WebHost
{
    int port;
    string pathBase = @"..\..\..\";
    HttpListener listener;

    public WebHost(int port)
    {
        this.port = port;
    }

    public void Run()
    {
        listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();
        Console.WriteLine($"Http server started at {port}");
        while (true)
        {
            var context = listener.GetContext();
            Task.Run(() => HandleRequest(context));
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var url = context.Request.RawUrl;
        var path = @$"{pathBase}{url!.Split("/").Last()}";
        var response = context.Response;
        StreamWriter streamWriter = new(response.OutputStream);
        try
        {
            var src = File.ReadAllText(path);
            streamWriter.WriteLine(src);
        }
        catch (Exception)
        {
            var src = File.ReadAllText(@$"{pathBase}404.html");
            streamWriter.WriteLine(src); ;
        }
        finally
        {
            streamWriter.Close();
        }
    }
}
