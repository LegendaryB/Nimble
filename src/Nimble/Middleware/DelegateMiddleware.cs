using System.Net;

namespace Nimble.Middleware;

internal class DelegateMiddleware : IMiddleware
{
    private readonly NimbleMiddlewareDelegate _middleware;
    
    internal DelegateMiddleware(NimbleMiddlewareDelegate middleware)
    {
        _middleware = middleware;
    }
    
    public Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        return _middleware.Invoke(
            ctx,
            next,
            cancellationToken);
    }
}