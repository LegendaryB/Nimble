using System.Net;

namespace Nimble.Middleware;

internal class RequestLoggingMiddleware : IMiddleware
{
    private readonly Func<MiddlewareContext, CancellationToken, Task> _loggerDelegate;
    private readonly Func<HttpListenerRequest, bool>? _filter;

    internal RequestLoggingMiddleware(
        Func<MiddlewareContext, CancellationToken, Task> loggerDelegate,
        Func<HttpListenerRequest, bool>? filter = null)
    {
        _loggerDelegate = loggerDelegate;
        _filter = filter;
    }
    
    public async Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (_filter?.Invoke(ctx.Request) == false)
        {
            await next(cancellationToken);
            return;
        }

        await next(cancellationToken);
        
        await _loggerDelegate.Invoke(
            ctx,
            cancellationToken);
    }
}