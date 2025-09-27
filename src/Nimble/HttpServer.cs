using System.Net;
using Nimble.Extensions;
using Nimble.Middleware;
using Nimble.Routing;

namespace Nimble;

public class HttpServer : IDisposable
{
    private readonly HttpListener _listener = new();
    private readonly List<IMiddleware> _middlewares = [];

    internal IRouter Router { get; set; } = new Router();
    
    public HttpServer(params string[] prefixes)
    {   
        var prefixesToAdd = prefixes
            .Where(prefix => !string.IsNullOrWhiteSpace(prefix));
        
        foreach (var prefix in prefixesToAdd)
            _listener.Prefixes.Add(prefix);
    }

    public HttpServer Use(NimbleMiddlewareDelegate middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        
        return Use(new DelegateMiddleware(middleware));
    }
    
    public HttpServer Use<TMiddleware>()
        where TMiddleware : IMiddleware, new() => Use(new TMiddleware());
    
    public HttpServer Use<TMiddleware>(TMiddleware middleware)
        where TMiddleware : IMiddleware
    {
        ArgumentNullException.ThrowIfNull(middleware);
        
        _middlewares.Add(middleware);

        return this;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            var (success, ctx) = await _listener.TryGetContextAsync();

            if (!success)
                break;

            await InvokeMiddlewareChainAsync(
                ctx!,
                cancellationToken);
        }

        _listener.Stop();
    }

    private async Task InvokeMiddlewareChainAsync(
        HttpListenerContext ctx,
        CancellationToken cancellationToken)
    {
        var index = 0;
        
        Func<CancellationToken, Task> next = null!;
        next = async ct =>
        {
            if (index < _middlewares.Count)
            {
                var middleware = _middlewares[index++];

                try
                {
                    await middleware.InvokeAsync(
                        ctx,
                        next,
                        ct);
                }
                catch
                {
                    await ctx.Response.RespondWithStatusCodeAsync(
                        HttpStatusCode.InternalServerError);
                }

                return;
            }

            await Router.RouteAsync(
                ctx.Request,
                ctx.Response,
                ct);
        };

        await next(cancellationToken);
    }

    public void Dispose()
    {
        ((IDisposable)_listener).Dispose();
    }
}