using System.Net;

namespace Nimble.Middleware;

internal class RequestLoggingMiddleware : IMiddleware
{
    private readonly Func<MiddlewareContext, CancellationToken, Task> _loggerDelegate;
    private readonly Func<HttpListenerRequest, bool> _predicate;

    internal RequestLoggingMiddleware(
        Func<MiddlewareContext, CancellationToken, Task> loggerDelegate,
        Func<HttpListenerRequest, bool> predicate)
    {
        _loggerDelegate = loggerDelegate;
        _predicate = predicate;
    }
    
    public async Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (!_predicate.Invoke(ctx.Request))
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