using System.Net;
using Nimble.Extensions;
using Nimble.Middleware;
using Nimble.Routing;

namespace Nimble;

public class HttpServer : IDisposable
{
    private readonly HttpListener _listener = new();
    private readonly List<IMiddleware> _middlewares = [];

    private Func<HttpListenerRequest, Task>? _onRequestReceivedCallback;
    private Func<HttpListenerResponse, Task>? _onResponseSentCallback;
    private Func<Exception, HttpListenerContext, Task>? _onUnhandledExceptionCallback;
    private Func<Task>? _onServerStartedCallback;
    private Func<Task>? _onServerStoppedCallback;
    
    internal IRouter Router { get; set; } = new Router();

    public HttpServer(bool blockHttpTrace = true)
        : this(blockHttpTrace, "http://localhost:5000") {}
    
    public HttpServer(string prefix, bool blockHttpTrace = true)
        : this(blockHttpTrace, prefix) {}
    
    public HttpServer(
        bool blockHttpTrace = true,
        params string[] prefixes)
    {
        foreach (var prefix in prefixes)
            _listener.Prefixes.Add(NormalizeAndValidatePrefix(prefix));

        if (blockHttpTrace)
            this.UseRequestBlocking();
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

        if (_onServerStartedCallback != null)
            await _onServerStartedCallback.Invoke();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var (success, ctx) = await _listener.TryGetContextAsync();

            if (!success)
                break;

            try
            {
                if (_onRequestReceivedCallback != null)
                    await _onRequestReceivedCallback.Invoke(ctx!.Request);

                await InvokeMiddlewareChainAsync(
                    ctx!,
                    cancellationToken);

                if (_onResponseSentCallback != null)
                    await _onResponseSentCallback(ctx!.Response);
            }
            catch (Exception ex)
            {
                if (_onUnhandledExceptionCallback != null)
                {
                    await _onUnhandledExceptionCallback.Invoke(
                        ex,
                        ctx!);

                    return;
                }

                await ctx!.Response.RespondWithStatusCodeAsync(
                    HttpStatusCode.InternalServerError);
            }
            finally
            {
                ctx?.Response.OutputStream.FlushAsync(cancellationToken);
                ctx?.Response.Close();
            }
        }
        
        _listener.Stop();
        
        if (_onServerStoppedCallback != null)
            await _onServerStoppedCallback.Invoke();
    }

    public HttpServer SetOnRequestReceivedCallback(Func<HttpListenerRequest, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _onRequestReceivedCallback = callback;
        return this;
    }
    
    public HttpServer SetOnResponseSentCallback(Func<HttpListenerResponse, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _onResponseSentCallback = callback;
        return this;
    }
    
    public HttpServer SetOnUnhandledExceptionCallback(Func<Exception, HttpListenerContext, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _onUnhandledExceptionCallback = callback;
        return this;
    }
    
    public HttpServer SetOnServerStartedCallback(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _onServerStartedCallback = callback;
        return this;
    }
    
    public HttpServer SetOnServerStoppedCallback(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _onServerStoppedCallback = callback;
        return this;
    }

    private async Task InvokeMiddlewareChainAsync(
        HttpListenerContext ctx,
        CancellationToken cancellationToken)
    {
        var middlewareCtx = new MiddlewareContext(ctx);
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
                        middlewareCtx,
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