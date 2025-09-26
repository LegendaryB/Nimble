using System.Net;
using Nimble.Routing;

namespace Nimble;

public class HttpServer : IDisposable
{
    private readonly HttpListener _listener = new();

    internal IRouter Router { get; set; } = new Router();
    
    public HttpServer(params string[] prefixes)
    {   
        var prefixesToAdd = prefixes
            .Where(prefix => !string.IsNullOrWhiteSpace(prefix));
        
        foreach (var prefix in prefixesToAdd)
            _listener.Prefixes.Add(prefix);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            var ctx = await _listener.GetContextAsync();
            
            await Router.RouteAsync(
                ctx.Request,
                ctx.Response,
                cancellationToken);
        }

        _listener.Stop();
    }

    public void Dispose()
    {
        ((IDisposable)_listener).Dispose();
    }
}