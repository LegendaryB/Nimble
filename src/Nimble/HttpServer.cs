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

    public HttpServer()
        : this("http://localhost:5000") {}
    
    public HttpServer(string prefix)
        : this([prefix]) {}
    
    public HttpServer(params string[] prefixes)
    {
        foreach (var prefix in prefixes)
            _listener.Prefixes.Add(NormalizeAndValidatePrefix(prefix));
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

            try
            {
                await InvokeMiddlewareChainAsync(
                    ctx!,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await ctx!.Response.RespondWithStatusCodeAsync(HttpStatusCode.InternalServerError);
            }
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
    
    private static string NormalizeAndValidatePrefix(string prefix)
    {
        var normalizedPrefix = NormalizePrefix(prefix);

        if (!Uri.TryCreate(
            normalizedPrefix,
            UriKind.Absolute,
            out var uri))
        {
            throw new ArgumentException(
                "Invalid prefix: Must be a valid absolute URI.",
                nameof(prefix));
        }

        return NormalizePrefixPort(
            normalizedPrefix,
            uri);
    }

    private static string NormalizePrefix(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        
        prefix = prefix.Trim();
        
        if (!prefix.EndsWith("/"))
            prefix += "/";

        if (!prefix.StartsWith("http://") &&
            !prefix.StartsWith("https://"))
        {
            prefix = "http://" + prefix;
        }

        return prefix;
    }

    private static string NormalizePrefixPort(
        string prefix,
        Uri uri)
    {
        return uri.IsDefaultPort 
            ? $"{uri.Scheme}://{uri.Host}:{(uri.Scheme == "https" ? 443 : 80)}/" 
            : prefix;
    }
    
    public void Dispose()
    {
        ((IDisposable)_listener).Dispose();
    }
}